using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool _gameIsActive;

    private int _bounces = 0;
    private int _balls = 0;
    private int _milestonesReached = 0;
    private int _lastTextShowIndex = 0;
    private float _bpsMultiplier = 1.0f;
    private float _elapsedTime = 1.0f;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip riser;
    [SerializeField] private AudioClip[] announcerClip;
    [SerializeField] private AudioClip[] headSpawnClip;
    [SerializeField] private AudioClip gameLostClip;

    [Header("Statistics")]
    [SerializeField] private int _score = 0; 
    [SerializeField] private int[] _scoreMilestones;

    [Header("Prefabs")]
    [SerializeField] private GameObject _speedChanger;
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private GameObject _countDownObject;

    [Header("References")]
    [SerializeField] private TextMesh _scoreText; 
    [SerializeField] private TextMesh _bouncesText;
    [SerializeField] private Animator[] _playerPortraits;
    [SerializeField] private Animator[] _textAnimators;
    [SerializeField] private GameObject _pressSpaceToStartObject;
    [SerializeField] private GameObject[] _playerObjects;
    [SerializeField] private GameObject[] _onScreenPopups;
    [SerializeField] private HighScores _highScoreManager;
    [SerializeField] private GameObject _controlSchemeImage;
    private Image _controls;
    private bool _controlsScreenIsActive = false;

    [System.Serializable]
    struct SpawnSetting
    {
        public GameObject itemPrefab;
        public bool spawnAfterScore;
        public float spawnInterval;
        public float spawnScoreInterval;
    }
    [SerializeField] private SpawnSetting[] itemSpawnSettings;

    private List<SpawnSetting> itemsToSpawnAfterScore = new List<SpawnSetting>();
    private List<GameObject> activePickups = new List<GameObject>();
    private List<GameObject> _deathImageList = new List<GameObject>();

    // UPDATES
    // ==============================================================================================================
    void Start()
    {
        _gameIsActive = false;
        _controls = _controlSchemeImage.GetComponent<Image>();

        if (PlayerPrefs.HasKey("Is4Player"))
        {
            if (PlayerPrefs.GetInt("Is4Player") == 1)
            {
                _playerObjects[2].SetActive(true);
                _playerObjects[3].SetActive(true);

                _playerPortraits[2].GetComponentInParent<SpriteRenderer>().enabled = true;
                _playerPortraits[3].GetComponentInParent<SpriteRenderer>().enabled = true;
            }
        }

        foreach (SpawnSetting toSpawn in itemSpawnSettings)
        {
            if (toSpawn.spawnAfterScore == false &&
                toSpawn.itemPrefab != null &&
                toSpawn.spawnInterval > 1)
            {
                StartCoroutine(StartPickupTimer(toSpawn.itemPrefab, toSpawn.spawnInterval));
            }
            else if (toSpawn.spawnAfterScore)
            {
                itemsToSpawnAfterScore.Add(toSpawn);
            }
        }
    }
    void LerpTimeScale(float t)
    {
        Time.timeScale = Mathf.Lerp(1, 0.5f, Time.deltaTime * 7);
    }
    void Update()
    {
        if (_gameIsActive) return;
        if (Input.GetAxisRaw("Submit") > 0)
        {
            ClearActivePickups();
            ClearDeathImageList();
            _countDownObject.SetActive(true);
            _pressSpaceToStartObject.SetActive(false);
            AudioHandler.instance.RandomizeEffects(riser);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if ((_controlsScreenIsActive)) { _controlsScreenIsActive = false; } 
            else                           { _controlsScreenIsActive = true;  }
            
        }
        SetControlsScreen();
    }

    private void SetControlsScreen()
    {
        if (_controlsScreenIsActive) { _controls.color = Color.Lerp(_controls.color, Color.white, Mathf.PingPong(Time.time, 0.75f)); }
        else                         { _controls.color = Color.Lerp(_controls.color, Color.clear, Mathf.PingPong(Time.time, 0.75f)); }
    }

    private void LateUpdate()
    {
        if (!_gameIsActive) return;
        _elapsedTime += Time.deltaTime;
        SetBpsMultiplierText();
    }

    // GAME STATE
    // ==============================================================================================================
    public void InitializeNewRound()
    {
        _gameIsActive = true;
        _elapsedTime = 1.0f;
        _score = 0;
        _balls = 0;
        _bounces = 0;
        _milestonesReached = 0;
        _scoreText.text = _score.ToString();
        _bouncesText.text = _bounces.ToString();

        SpawnBall();
    }
    public void ClearActivePickups()
    {
        foreach (GameObject pickup in activePickups)
        {
            Destroy(pickup);
        }
        activePickups.Clear();
    }
    public void RoundLost()
    {
        if (_score > 0)
        {
            _highScoreManager.SubmitScore(_score);
        }

        _gameIsActive = false;
        AudioHandler.instance.SoundQueue(AudioHandler.instance.effectsSource, gameLostClip);
        _pressSpaceToStartObject.SetActive(true);
    }

    // SCORE AND STATS
    // ==============================================================================================================
    public void IncreaseScore(int amount)
    {
        _score += (int)(500f * _bpsMultiplier);
        _scoreText.text = _score.ToString();

        if (_milestonesReached < _scoreMilestones.Length)
        {
            if (_score >= _scoreMilestones[_milestonesReached])
            {
                _milestonesReached++;

                int nextTextIndex = 0;
                do
                {
                    nextTextIndex = Random.Range(0, 5);
                } while (nextTextIndex == _lastTextShowIndex);

                switch (nextTextIndex)
                {
                    case 0:
                        TriggerPopupText(_textAnimators[0]);
                        AudioHandler.instance.RandomizeEffects(announcerClip[0]);
                        break;
                    case 1:
                        TriggerPopupText(_textAnimators[1]);
                        AudioHandler.instance.RandomizeEffects(announcerClip[0]);
                        break;
                    case 2:
                        TriggerPopupText(_textAnimators[2]);
                        AudioHandler.instance.RandomizeEffects(announcerClip[1]);
                        break;
                    case 3:
                        TriggerPopupText(_textAnimators[3]);
                        AudioHandler.instance.RandomizeEffects(announcerClip[1]);
                        break;
                }
            }
        }

        foreach (SpawnSetting toSpawn in itemsToSpawnAfterScore)
        {
            if (_score % toSpawn.spawnScoreInterval == 0)
            {
                SpawnPickup(toSpawn.itemPrefab);
            }
        }
    }
    public void DecreaseScore(int amount)
    {
        _score = _score - amount < 0 ? 0 : _score - amount;
        _scoreText.text = _score.ToString();
    }
    public void ResetScore()
    {
        _score = 0;
        _scoreText.text = "0";
    }

    // SPAWNING
    // ==============================================================================================================
    public void SpawnPickup(GameObject pickupToSpawn, float timeBetweenSpawns)
    {
        if (_gameIsActive)
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            Vector2 randomPos = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized * Random.Range(0f, 4f);
            GameObject spawnedPickup = Instantiate(pickupToSpawn, randomPos, Quaternion.identity);
            if (spawnedPickup.GetComponent<MultiBallScript>())
            {
                spawnedPickup.GetComponent<MultiBallScript>().gm = this;
            }
            activePickups.Add(spawnedPickup);
        }
        StartCoroutine(StartPickupTimer(pickupToSpawn, timeBetweenSpawns));
    }
    public void SpawnPickup(GameObject pickupToSpawn)
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        Vector2 randomPos = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized * Random.Range(0f, 4f);
        GameObject spawnedPickup = Instantiate(pickupToSpawn, randomPos, Quaternion.identity);
        activePickups.Add(spawnedPickup);
    }
    public void AddDeathImageToList(GameObject image)
    {
        _deathImageList.Add(image);
    }
    private void ClearDeathImageList()
    {
        foreach(GameObject image in _deathImageList)
        {
            Destroy(image);
        }
        _deathImageList.Clear();
    }
    IEnumerator StartPickupTimer(GameObject pickupToSpawn, float timeBetweenSpawns)
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        SpawnPickup(pickupToSpawn, timeBetweenSpawns);
    }
    
    // BALL
    // ==============================================================================================================
    public void SpawnBall()
    {
        Ball ballTemp = Instantiate(_ballPrefab, Vector3.zero, Quaternion.identity)?.GetComponent<Ball>();
        ballTemp.gm = this;
        _balls++;

        AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, headSpawnClip);
    }

    public void IncreaseBps(int amount)
    {
        _bounces += amount;
    }
    public void DecreaseActiveBalls()
    {
        _balls--;
        if (_balls <= 0)
        {
            RoundLost();
        }
    }
    public void SetBpsMultiplierText()
    {
        _bpsMultiplier = 1 + (_bounces / _elapsedTime);
        _bouncesText.text = (_bpsMultiplier).ToString("0.0");
    }

    // UI
    // ==============================================================================================================
    public void AnimatePortrait(Player playerIdentifier)
    {
        switch (playerIdentifier.GetPlayerIndex())
        {
            case 0:
                _playerPortraits[0].Play("YellowAnim");
                break;
            case 1:
                _playerPortraits[1].Play("GreenAnim");
                break;
            case 2:
                _playerPortraits[2].Play("RedAnim");
                break;
            case 3:
                _playerPortraits[3].Play("BlueAnim");
                break;
            default:
                Debug.Log("playerIdentifier invalid");
                break;
        }
    }

    public void TriggerPopupText(Animator animator)
    {
        animator.SetTrigger("PlayAnim");
        foreach (Animator anim in _playerPortraits)
        {
            anim.SetTrigger("PlayAnim");
        }
    }
    public void SetCountDown()
    {
        InitializeNewRound();
        _countDownObject.SetActive(false);
    }
}