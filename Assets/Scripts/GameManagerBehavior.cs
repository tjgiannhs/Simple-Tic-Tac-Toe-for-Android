using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

//as a challenge, all of the code below is written in a way that allows for different dimensions to the table other
//than 3x3 but I didn't implement the player changing the board dimensions in the settings for the purposes of this test


public class GameManagerBehavior : MonoBehaviour
{
    [SerializeField] GameObject gameboard;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject restartButton;
    int numberOfColumns, numberOfRows, playerWins, comWins;
    bool playerCanAct = true;
    bool playerGoesSecond = false;
    AudioSource myAudio;
    [SerializeField] AudioClip victorySound;
    
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        playerWins = comWins = 0;
        numberOfColumns = gameboard.transform.GetChild(0).childCount;
        numberOfRows = gameboard.transform.childCount;
        ClearBoard();
        Vibration.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public bool CanPlayerAct()
    {
        return playerCanAct;
    }
    public void ClearBoard()
    {
        for(int i=0; i<numberOfRows; i++)
        {
            for(int j = 0; j<numberOfColumns; j++){
                gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        if(playerGoesSecond)
        {
            StartCoroutine(ComTurn(true));
        }        
        playerGoesSecond = !playerGoesSecond;//for next game
    }

    bool CheckIfBoardFull()
    {
        for(int i=0; i<numberOfRows; i++)
        {
            for(int j = 0; j<numberOfColumns; j++){
                if(gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "") return false;
            }
        }
        print("Game End");//no more moves left
        return true;
    }

    bool CheckHorizontalWin(string s)
    {
        for(int i=0; i<numberOfRows; i++)
        {
            for(int j=1; j<numberOfColumns-1; j++)
            {
                //Checking a square and its two adjacent ones at the same time
                if(gameboard.transform.GetChild(i).GetChild(j-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                    gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                    gameboard.transform.GetChild(i).GetChild(j+1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                )
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckVerticalWin(string s)
    {
        for(int i=1; i<numberOfRows-1; i++)
        {
            for(int j=0; j<numberOfColumns; j++)
            {
                //Checking a square and its two adjacent ones at the same time
                if(gameboard.transform.GetChild(i-1).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                    gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                    gameboard.transform.GetChild(i+1).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                )
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckDiagonalWin(string s)
    {
        //for the upper-left-to-down-right diagonal
        for(int i=1; i<Mathf.Min(numberOfRows,numberOfColumns)-1; i++)
        {
            //Checking three diagonally neighbor squares at the same time
            if(gameboard.transform.GetChild(i-1).GetChild(i-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                gameboard.transform.GetChild(i).GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                gameboard.transform.GetChild(i+1).GetChild(i+1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
            )
            {
                return true;
            }
        }
        //for the upper-right-to-down-left diagonal
        for(int i=1; i<Mathf.Min(numberOfRows,numberOfColumns)-1; i++)
        {
            //Checking three diagonally neighbor squares at the same time
            if(gameboard.transform.GetChild(i-1).GetChild(i+1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                gameboard.transform.GetChild(i).GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s &&
                gameboard.transform.GetChild(i+1).GetChild(i-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
            )
            {
                return true;
            }
        }
        return false;
    }

    bool CheckIfXWins()
    {
        if(CheckHorizontalWin("X")) return true;
        if(CheckVerticalWin("X")) return true;
        if(CheckDiagonalWin("X")) return true;
        return false;
    }

    bool CheckIfOWins()
    {
        if(CheckHorizontalWin("O")) return true;
        if(CheckVerticalWin("O")) return true;
        if(CheckDiagonalWin("O")) return true;
        return false;
    }

    public bool CheckIfGameEnd()
    {
        if(CheckIfXWins())
        {
            print("X wins");
            PlayVictorySound();
            PlayBigVibration();
            playerWins++;
            PlayerPrefs.SetInt("TotalWins",PlayerPrefs.GetInt("TotalWins",0)+1);
            ChangeScoreText(true);
            restartButton.SetActive(true);
            return true;
        
        }else if(CheckIfOWins()){
            print("O wins");
            PlayBigVibration();
            comWins++;
            PlayerPrefs.SetInt("TotalLosses",PlayerPrefs.GetInt("TotalLosses",0)+1);
            ChangeScoreText(false);
            restartButton.SetActive(true);
            return true;

        }else if (CheckIfBoardFull())
        {
            PlayerPrefs.SetInt("TotalDraws",PlayerPrefs.GetInt("TotalDraws",0)+1);
            PlayBigVibration();
            restartButton.SetActive(true);
            return true;
        }
        return false;
    }

    void ChangeScoreText(bool playerWon)
    {
        scoreText.text = "You - Com:\n"+playerWins+" : "+comWins;
    }

    void ComPlayRandomMove()
    {
        int randomSquareRowIndex = Random.Range(0, numberOfRows);
        int randomSquareColumnIndex = Random.Range(0, numberOfColumns);

        Transform squareToCheck = gameboard.transform.GetChild(randomSquareRowIndex).GetChild(randomSquareColumnIndex).GetChild(0).GetChild(0);

        while (squareToCheck.GetComponent<TextMeshProUGUI>().text != "")
        {
            randomSquareRowIndex = Random.Range(0, numberOfRows);
            randomSquareColumnIndex = Random.Range(0, numberOfColumns);
            squareToCheck = gameboard.transform.GetChild(randomSquareRowIndex).GetChild(randomSquareColumnIndex).GetChild(0).GetChild(0);        
        }

        squareToCheck.GetComponent<TextMeshProUGUI>().text = "O";
    }

    //an original algorithm so that the bot plays optimally, thus being unable to lose
    //constructed using the logic of avoiding grave mistakes first, making good moves second
    //and polished after lots of playtesting
    void ComPlayPerfectly(bool firstTurn)
    {
        int targetSquareIndex;
        Transform targetSquare;

        //first turn should be taking the center
        if(firstTurn)
        {
            if(!ComPlayCenter())
            {
                //in case there is not a single center square
                targetSquare = gameboard.transform.GetChild(Random.Range(numberOfRows/2,numberOfRows/2+2)).GetChild(Random.Range(numberOfColumns/2,numberOfColumns/2+2)).GetChild(0).GetChild(0);
                targetSquare.GetComponent<TextMeshProUGUI>().text = "O";
            }

        }else
        {
            int solutionIndex;//used as a supporting variable, both for columns and rows
            string s;
            solutionIndex = CheckIfHorizontalWinIn1("O");
            if(solutionIndex!=-1)
            {
                ComPlayHorizontal(solutionIndex);
                return;
            }

            solutionIndex = CheckIfVerticalWinIn1("O");
            if(solutionIndex!=-1)
            {
                ComPlayVertical(solutionIndex);
                return;
            }

            s = CheckIfDiagonalWinIn1("O");
            if(s!="")
            {
                ComPlayDiagonal(s[0]+"",(int)s[1]);
                return;
            }

            solutionIndex = CheckIfHorizontalWinIn1("X");
            if(solutionIndex!=-1)
            {
                ComPlayHorizontal(solutionIndex);
                return;
            }

            solutionIndex = CheckIfVerticalWinIn1("X");
            if(solutionIndex!=-1)
            {
                ComPlayVertical(solutionIndex);
                return;
            }

            s = CheckIfDiagonalWinIn1("X");
            if(s!="")
            {
                ComPlayDiagonal(s[0]+"",(int)s[1]);
                return;
            }


            if(ComPlayCenter()) return;

            //trapping allows one player to have two options to win the game in one move, making their win unpreventable by the opponent
            //if player has the center and com has 2 corners with more corners available try to trap him
            if(numberOfRows==3 & numberOfColumns==3)
            {
                if(HasTheCenter("X") && NumberOfCapturedCorners("O")==2 && NumberOfCapturedCorners("X")!=2)
                {
                    if(ComPlayCorner()) return;
                }
            }

            //if com has the center and player has 2 corners with more corners available prevent trap
            if(numberOfRows==3 & numberOfColumns==3)
            {
                if(HasTheCenter("O") && NumberOfCapturedCorners("X")==2 && NumberOfCapturedCorners("O")!=2)
                {
                    if(ComPlayNonCorner()) return;
                }
            }

            //com prefers playing on squares that belong to the diagonals because I read they are more useful
            if(!CheckIfDiagonalFilled("l")||!CheckIfDiagonalFilled("r"))
            {
                s = Random.Range(0,2)>0.5f?"l":"r";
                solutionIndex = Random.Range(0,Mathf.Abs(numberOfColumns-numberOfRows)+1);

                while(!ComPlayDiagonal(s,solutionIndex))
                {
                    s = Random.Range(0,2)>0.5f?"l":"r";
                    solutionIndex = Random.Range(0,Mathf.Abs(numberOfColumns-numberOfRows)+1);
                }

                return;
            }

            ComPlayRandomMove();

        }
    }

    int NumberOfCapturedCorners(string s)
    {
        //manually checking each of the 4 corners
        int n = 0;
        n += gameboard.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s?1:0;
        n += gameboard.transform.GetChild(0).GetChild(numberOfColumns-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s?1:0;
        n += gameboard.transform.GetChild(numberOfRows-1).GetChild(numberOfColumns-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s?1:0;
        n += gameboard.transform.GetChild(numberOfRows-1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s?1:0;
        print(s+" has "+n+" corners");
        return n;
    }

    bool HasTheCenter(string s)
    {
        //a single center square doesn't exist
        if(numberOfColumns%2==0 && numberOfRows%2==0) return false;
        
        TextMeshProUGUI centerSquare = gameboard.transform.GetChild(numberOfRows/2).GetChild(numberOfColumns/2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        print(centerSquare.text+" has the center "+s);
        if(centerSquare.text == s) return true;

        return false;
    }

    void ComPlayNonDiagonal()
    {

    }

    bool ComPlayCorner()
    {
        List<UnityEngine.Vector2> cornerSquaresIndexes = new List<UnityEngine.Vector2>{
            new UnityEngine.Vector2(0,0),
            new UnityEngine.Vector2(0,numberOfColumns-1),
            new UnityEngine.Vector2(numberOfRows-1,numberOfColumns-1),
            new UnityEngine.Vector2(numberOfRows-1,0)
            };

        List<UnityEngine.Vector2> approvedCornersIndexes = new List<UnityEngine.Vector2>();

        int horizontalAdjacentIndex, verticalAdjacentIndex;
        TextMeshProUGUI horizontalAdjacent, verticalAdjacent;

        //com shouldn't choose a corner that's adjacent to any Xs
        foreach (UnityEngine.Vector2 v in cornerSquaresIndexes)
        {
            horizontalAdjacentIndex = v[1]+1==numberOfColumns?(int)v[1]-1:(int)v[1]+1;
            horizontalAdjacent = gameboard.transform.GetChild((int)v[0]).GetChild(horizontalAdjacentIndex).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    
            verticalAdjacentIndex = v[0]+1==numberOfRows?(int)v[0]-1:(int)v[0]+1;
            verticalAdjacent = gameboard.transform.GetChild(verticalAdjacentIndex).GetChild((int)v[1]).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            if(horizontalAdjacent.text == "" || verticalAdjacent.text == "")
                approvedCornersIndexes.Add(v);
        }
        
        if(approvedCornersIndexes.Count==0) return false;

        UnityEngine.Vector2 selectedCornerIndex = approvedCornersIndexes[Random.Range(0,approvedCornersIndexes.Count)];
        gameboard.transform.GetChild((int)selectedCornerIndex[0]).GetChild((int)selectedCornerIndex[1]).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "O";

        return true;
    }

    bool CheckIfNonCornerAvailable()
    {
        for(int i=0; i<numberOfRows; i++)
        {
            for(int j=0; j<numberOfColumns; j++)
            {
                //excluding the 4 corners
                if(!(i==0 && j==0) &&
                   !(i==0 && j==numberOfColumns-1) &&
                   !(i==numberOfRows-1 && j==0) &&
                   !(i==numberOfRows-1 && j==numberOfColumns-1))
                {
                    if(gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                        return true;
                }
            }
        }

        return false;
    }

    bool ComPlayNonCorner()
    {
        if(CheckIfNonCornerAvailable())
        {
            int i = Random.Range(0,numberOfRows);
            int j = Random.Range(0,numberOfColumns);
            TextMeshProUGUI targetSquare = gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            while( (i==0 && j==0) ||
                   (i==0 && j==numberOfColumns-1) ||
                   (i==numberOfRows-1 && j==0) ||
                   (i==numberOfRows-1 && j==numberOfColumns-1) ||
                   targetSquare.text != "")
            {
                i = Random.Range(0,numberOfRows);
                j = Random.Range(0,numberOfColumns);

                targetSquare = gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            targetSquare.text = "O";
            return true;
        }
        return false;
    }

    bool ComPlayCenter()
    {
        //a single center square doesn't exist
        if(numberOfColumns%2==0 && numberOfRows%2==0) return false;
        
        TextMeshProUGUI centerSquare = gameboard.transform.GetChild(numberOfRows/2).GetChild(numberOfColumns/2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if(centerSquare.text == "")
        {
            centerSquare.text = "O";
            return true;
        }
        return false;
    }

    void ComPlayHorizontal(int rowIndex)
    {
        TextMeshProUGUI targetSquareText;

        for(int j=0; j<numberOfColumns; j++)
        {
            targetSquareText = gameboard.transform.GetChild(rowIndex).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            if(targetSquareText.text == "")
            {
                targetSquareText.text = "O";
                return;
            }
        }
    }
    void ComPlayVertical(int columnIndex)
    {
        TextMeshProUGUI targetSquareText;

        for(int i=0; i<numberOfRows; i++)
        {
            targetSquareText = gameboard.transform.GetChild(i).GetChild(columnIndex).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            if(targetSquareText.text == "")
            {
                targetSquareText.text = "O";
                return;
            }
        }
    }

    bool ComPlayDiagonal(string diagonalType, int shift)
    {
        int repeats = Mathf.Min(numberOfRows,numberOfColumns);
        TextMeshProUGUI targetSquareText;
        //in case of non-square boards there can be more than one diagonals with maximum length
        int rowshift = numberOfRows>numberOfColumns?shift:0;
        int columnshift = numberOfColumns>numberOfRows?shift:0;

        if(diagonalType=="l")
        {
            for(int i=0; i<repeats; i++)
            {
                targetSquareText = gameboard.transform.GetChild(i+rowshift).GetChild(i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                if(targetSquareText.text == "")
                {
                    targetSquareText.text = "O";
                    return true;
                }
            }
        }else //if(diagonalType=="r")
        {
            for(int i=repeats-1; i>=0; i--)
            {
                targetSquareText = gameboard.transform.GetChild(i+rowshift).GetChild(repeats-1-i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                if(targetSquareText.text == "")
                {
                    targetSquareText.text = "O";
                    return true;
                }
            }
        }

        return false;
    }

    int CheckIfHorizontalWinIn1(string s)
    {
        for(int i=0; i<numberOfRows; i++)
        {
            for(int j=2; j<numberOfColumns; j++)
            {
                if(gameboard.transform.GetChild(i).GetChild(j-2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =="" 
                && gameboard.transform.GetChild(i).GetChild(j-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s)
                    return i;

                if(gameboard.transform.GetChild(i).GetChild(j-2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s 
                && gameboard.transform.GetChild(i).GetChild(j-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==""
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s)
                    return i;

                if(gameboard.transform.GetChild(i).GetChild(j-2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s 
                && gameboard.transform.GetChild(i).GetChild(j-1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text ==s
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =="")
                    return i;

            }
        }
        return -1;
    }
    int CheckIfVerticalWinIn1(string s)
    {
        for(int i=2; i<numberOfRows; i++)
        {
            for(int j=0; j<numberOfColumns; j++)
            {
                if(gameboard.transform.GetChild(i-2).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i-1).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return j;

                if(gameboard.transform.GetChild(i-2).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return j;

                if(gameboard.transform.GetChild(i-2).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                    return j;
            }
        }
        return -1;
    }
    string CheckIfDiagonalWinIn1(string s)
    {
        int repeats = Mathf.Min(numberOfRows,numberOfColumns);
        //in case of different number of rows and columns we'll have to check the board multiple times for diagonal pairs by shifting the process down or to the right
        int shift = Mathf.Abs(numberOfColumns-numberOfRows);
        int rowshift, columnshift;

        //left diagonal
        for(int i=2; i<repeats; i++)
        {
            for(int k=0; k<=shift; k++)
            {
                rowshift = numberOfRows>numberOfColumns?k:0;
                columnshift = numberOfColumns>numberOfRows?k:0;

                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(i-2+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i+rowshift).GetChild(i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return "l"+k;

                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(i-2+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i+rowshift).GetChild(i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return "l"+k;

                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(i-2+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i+rowshift).GetChild(i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                    return "l"+k;
            }
        }//right diagonal
        for(int i=repeats-1; i>1; i--)
        {
            for(int k=0; k<=shift; k++)
            {
                rowshift = numberOfRows>numberOfColumns?k:0;
                columnshift = numberOfColumns>numberOfRows?k:0;
                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(repeats-i+1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(repeats-i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i+rowshift).GetChild(repeats-i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return "r"+k;
                    
                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(repeats-i+1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(repeats-i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == ""
                && gameboard.transform.GetChild(i+rowshift).GetChild(repeats-i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s)
                    return "r"+k;

                if(gameboard.transform.GetChild(i-2+rowshift).GetChild(repeats-i+1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i-1+rowshift).GetChild(repeats-i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == s
                && gameboard.transform.GetChild(i+rowshift).GetChild(repeats-i-1+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                    return "r"+k;
            }
        }
        return "";
    }

    bool CheckIfDiagonalFilled(string s)
    {
        int repeats = Mathf.Min(numberOfRows,numberOfColumns);
        //in case of different number of rows and columns we'll have to check the board multiple times for diagonal pairs by shifting the process down or to the right
        int shift = Mathf.Abs(numberOfColumns-numberOfRows);
        int rowshift, columnshift;

        if(s == "l")
        {
            for(int i=0; i<repeats; i++)
            {
                for(int k=0; k<=shift; k++)
                {
                    rowshift = numberOfRows>numberOfColumns?k:0;
                    columnshift = numberOfColumns>numberOfRows?k:0;

                    if(gameboard.transform.GetChild(i+rowshift).GetChild(i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                        return false;
                }
            }
            return true;
        }else//if(s == "r")
        {
            for(int i=repeats-1; i>=0; i--)
            {
                for(int k=0; k<=shift; k++)
                {
                    rowshift = numberOfRows>numberOfColumns?k:0;
                    columnshift = numberOfColumns>numberOfRows?k:0;

                    if(gameboard.transform.GetChild(i+rowshift).GetChild(repeats-1-i+columnshift).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text == "")
                        return false;
                }
            }
            return true;
        }
    }

    IEnumerator ComTurn(bool firstTurn)
    {
        playerCanAct = false;

        if(firstTurn) yield return new WaitForSeconds(0.05f);
        else yield return new WaitForSeconds(Random.Range(0.25f,1.05f));

        int d = PlayerPrefs.GetInt("GameDifficulty");
        
        //based on the difficulty level COM is more likely to play the best move each turn instead of playing randomly
        //with the final difficulty level being unbeatable and the first one being completely random
        if(d==0)
            ComPlayRandomMove();
        else if(Random.Range(0,4)<d)
            ComPlayPerfectly(firstTurn);
        else
            ComPlayRandomMove();

        playerCanAct = true;

        CheckIfGameEnd();
    }

    public void AfterPlayerAction()
    {
        PlayClickSound();
        PlaySmallVibration();
        if(!CheckIfGameEnd())
        {
            StartCoroutine(ComTurn(false));
        }

    }


    public void PlayBigVibration()
    {
        if(PlayerPrefs.GetInt("Vibration",1)==0) return;

        #if UNITY_IPHONE
            Vibration.VibratePeek();    
        #endif
            
        #if UNITY_ANDROID
            Vibration.Vibrate (65);
        #endif
    }
    public void PlaySmallVibration()
    {
        if(PlayerPrefs.GetInt("Vibration",1)==0) return;

        #if UNITY_IPHONE
            Vibration.VibratePop();    
        #endif

        #if UNITY_ANDROID
            Vibration.Vibrate (15);
        #endif
    }
    void PlayVictorySound()
    {
        if(PlayerPrefs.GetInt("Sounds",1)==1) myAudio.PlayOneShot(victorySound, 0.3f);
    }

    public void PlayClickSound()
    {
        if(PlayerPrefs.GetInt("Sounds",1)==1) myAudio.Play();
    }
}