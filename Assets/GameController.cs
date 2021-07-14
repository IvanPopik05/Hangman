using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public enum GameStatus 
{
    Won,
    Lost,
    InProgress,
    NotStarted
}

public enum IsGame 
{
    Game,
    Menu
}

public class GameController : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject menuPanel;
    public Text wordText;
    public Button[] buttons;
    public Sprite[] sprites;
    public int triesCounter = 8;
    [Header("HangmanSprites")]
    public GameObject[] HangmanSprites;
    [Header("Data")]
    public DataGame[] dataGames;

    private int wordLength;
    private bool[] openIndexes;
    private DataGame data;
    private string words = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ";
    private List<string> dataType = new List<string>();

    public GameStatus GameStatus { get; set; } = GameStatus.NotStarted;
    public IsGame IsGame { get; set; } = IsGame.Menu;
    public string Word { get; private set; }
    private void Start()
    {
        if (IsGame == IsGame.Game) 
        {
            gamePanel.SetActive(true);
            menuPanel.SetActive(false);
            GenerateWord();
            wordText.text = GenerateField();
        }

    }

        private IEnumerator NextLevel(float waitTime)
        {
            while (GameStatus == GameStatus.Won || GameStatus == GameStatus.Lost)
            {
                yield return new WaitForSeconds(waitTime);
                IsGame = IsGame.Game;
                GameStatus = GameStatus.NotStarted;
                triesCounter = 8;
                for (int i = 0; i < HangmanSprites.Length; i++)
                {
                    HangmanSprites[i].SetActive(false);
                }
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = sprites[2];
                }
                GenerateWord();
                wordText.text = GenerateField();
            }
        }

    //private void NextLevel(int timer) 
    //{
    //    IsGame = IsGame.Game;
    //    GameStatus = GameStatus.NotStarted;
    //    for (int i = 0; i < buttons.Length; i++)
    //    {
    //        buttons[i].interactable = true;
    //        buttons[i].image.sprite = sprites[2];
    //    }
    //    GenerateWord();
    //    wordText.text = GenerateField();
    //}
    public void ThemeButtons(int index) 
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        data = dataGames[index];
        for (int i = 0; i < data.titles.Length; i++)
        {
            dataType.Add(data.titles[i]);
        }
        IsGame = IsGame.Game;
        Start();
    }
    public string GenerateField() 
    {
        string fieldEmpty = string.Empty;
        for (int i = 0; i < wordLength; i++)
        {
            fieldEmpty += "_ ";
        }
        GameStatus = GameStatus.InProgress;
        return fieldEmpty;
    }
    public string GenerateWord() 
    {
        int randomIndex = UnityEngine.Random.Range(0,dataType.Count);
        Word = dataType[randomIndex];
        dataType.RemoveAt(randomIndex);
        openIndexes = new bool[Word.Length];
        wordLength = Word.Length;
        return Word;
    }

    public string GuessLetter(char letter, int indexLetter) 
    {
        if (triesCounter < 0)
        {
            Debug.Log("У вас закончились попытки");
        }
        if (GameStatus != GameStatus.InProgress)
        {
            Debug.Log("Вы уже не в игре");
        }
        bool openAny = false;
        string result = string.Empty;
        for (int i = 0; i < Word.Length; i++)
        {
            if (Word[i] == letter)
            {
                openIndexes[i] = true;
                openAny = true;
            }
            if (openIndexes[i])
            {
                result += $"{Word[i]} ";
            }
            else
            {
                result += "_ ";
            }
        }
        if (!openAny)
        {
            triesCounter--;
            switch (triesCounter)
            {
                case 7:
                    HangmanSprites[0].SetActive(true);
                    break;
                case 6:
                    HangmanSprites[1].SetActive(true);
                    break;
                case 5:
                    HangmanSprites[2].SetActive(true);
                    break;
                case 4:
                    HangmanSprites[3].SetActive(true);
                    break;
                case 3:
                    HangmanSprites[4].SetActive(true);
                    break;
                case 2:
                    HangmanSprites[5].SetActive(true);
                    break;
                case 1:
                    HangmanSprites[6].SetActive(true);
                    break;
                case 0:
                    HangmanSprites[7].SetActive(true);
                    break;
                default:
                    Debug.Log("Нет спрайта");
                    break;
            }
            buttons[indexLetter].interactable = false;
            buttons[indexLetter].image.sprite = sprites[0];
        }
        else
        {
            buttons[indexLetter].image.sprite = sprites[1];
            buttons[indexLetter].interactable = false;
        }
        return result;
    }

    private bool isWin()
    {
        for (int i = 0; i < openIndexes.Length; i++)
        {
            if (openIndexes[i] == false) 
            {
                return false;
            }
        }
        return true;
    }

    private char FindLetter(int indexLetter) 
    {
        return words[indexLetter];
    }
    public void ClickWord(int indexLetter) 
    {
        char letter = FindLetter(indexLetter);
        wordText.text = GuessLetter(letter, indexLetter);
        if (isWin())
        {
            GameStatus = GameStatus.Won;
            Debug.Log("Ты победил");
            StartCoroutine(NextLevel(2));
        }
        else if (triesCounter <= 0)
        {
            GameStatus = GameStatus.Lost;
            Debug.Log("Ты проиграл");
            wordText.text = EnterWords();
            StartCoroutine(NextLevel(2));
        }
    }
    private string EnterWords() 
    {
        string result = string.Empty;
        for (int i = 0; i < Word.Length; i++)
        {
            result += $"{Word[i]} ";
        }
        return result;
    }
}
