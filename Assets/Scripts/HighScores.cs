using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScores : MonoBehaviour
{
    [SerializeField]
    private Text[] HighScoresTexts = new Text[5];

    private int[] topScores = new int[5];

    [ContextMenu("Delete all scores")]
    void ResetScores()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("Score1");
        PlayerPrefs.DeleteKey("Score2");
        PlayerPrefs.DeleteKey("Score3");
        PlayerPrefs.DeleteKey("Score4");
    }

    void Start()
    {
        GetScores();
        PrintScoresToUI();
    }

    void GetScores()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            topScores[0] = PlayerPrefs.GetInt("HighScore");
        }

        for (int i = 1; i < 5; i++)
        {
            if (PlayerPrefs.HasKey("Score" + i))
            {
                topScores[i] = PlayerPrefs.GetInt("Score" + i);
            }
        }
    }

    void SaveScores()
    {
        PlayerPrefs.SetInt("HighScore", topScores[0]);

        for (int i = 1; i < 5; i++)
        {
            PlayerPrefs.SetInt("Score" + i, topScores[i]);
        }

        PlayerPrefs.Save();
    }

    void PrintScoresToUI()
    {
        for (int i = 0; i < topScores.Length; i++)
        {
            if (topScores[i] == 0)
            {
                HighScoresTexts[i].text = "No Score";
            }
            else
            {
                HighScoresTexts[i].text = topScores[i].ToString();
            }
        }
    }

    public void SubmitScore(int score)
    {
        for (int i = 0; i < topScores.Length; i++)
        {
            if (score > topScores[i])
            {
                for (int j = HighScoresTexts.Length - 1; j >= i; j--)
                {
                    if (j - 1 >= 0)
                    {
                        topScores[j] = topScores[j - 1];
                    }
                }

                topScores[i] = score;

                PrintScoresToUI();
                SaveScores();
                return;
            }
        }
    }
}
