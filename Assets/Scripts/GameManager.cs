using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Static so all GameManagers have one same value in these variables
    [Header("Game Elements")]
    public static GameObject myPlayer;
    public static List<GameObject> wallList = new List<GameObject>();
    public static List<GameObject> enemyList = new List<GameObject>();
    public static List<GameObject> bulletList = new List<GameObject>();
    public static List<GameObject> weaponList = new List<GameObject>();

    [Header("UI Elements")]
    public TMP_Text playerHPText;
    public TMP_Text enemiesLeftText;
    public TMP_Text levelText;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject gameScreen;

    [Header("Level Maker Assets")]
    public int levelNumber; //which level are we on?
    public Texture2D[] levels; //level png images
    public CinemachineVirtualCamera levelCamera;

    public Color playerColor; //we will define this color in unity
    public GameObject playerPrefab;

    public Color enemyColor;  //Optional is to make each enemy a different color
    public GameObject[] enemyPrefabs; //Enemies are randomly selected from array

    public Color greenWallColor;
    public GameObject greenWallPrefab;
    public Color blueWallColor;
    public GameObject blueWallPrefab;
    public Color orangeWallColor;
    public GameObject orangeWallPrefab;

    public Color weaponColor;
    public GameObject[] weaponPrefabs;

    void Awake()
    {
        //Set the Title screen to be active when the game is first played, all other screens are inactive
        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        gameScreen.SetActive(false);
    }

    void LateUpdate()
    {
        //check if the player is alive and there are not enemies in our list
        if (myPlayer && enemyList.Count == 0)
        {
            enemiesLeftText.text = "Enemies Left: 0";
            victoryScreen.SetActive(true);
        }
        //if player is destroyed (no more reference) and there's still enemies around
        else if (myPlayer == null && enemyList.Count > 0)
        {
            gameOverScreen.SetActive(true);
        }
        else if (myPlayer && enemyList.Count > 0)
        {
            //update text elements while player is alive and there are enemies
            enemiesLeftText.text = "Enemies Left: " + enemyList.Count;
            playerHPText.text = "HP: " + Mathf.RoundToInt(myPlayer.GetComponent<Player>().hp);
            levelText.text = "Level: " + levelNumber;
        }
    }

    //Start Game Method deactivates all screens except the game Screen
    public void StartGame()
    {
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        gameScreen.SetActive(true);
        levelNumber = 0;
        CreateLevel(levelNumber);
    }

    private void ResetGame()
    {
        //go through all gameobject lists and destroy them from scene
        for (int i = 0; i < wallList.Count; i++)
        {
            Destroy(wallList[i]);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            Destroy(enemyList[i]);
        }
        for (int i = 0; i < bulletList.Count; i++)
        {
            Destroy(bulletList[i]);
        }
        for (int i = 0; i < weaponList.Count; i++)
        {
            Destroy(weaponList[i]);
        }
        //lists in scripts must also be emptied to avoid null references
        wallList.Clear();
        enemyList.Clear();
        bulletList.Clear();
        weaponList.Clear();
        Destroy(myPlayer);
    }

    public void CreateLevel(int levelNumber)
    {
         //Clear out the old level (empty lists, destroy previous GameObjects in scene)
        ResetGame();
        //if the levelNumber with within our total level array count
        if (levelNumber < levels.Length)
        {
            //Get a level image from our level array based on what levelNumber
            Texture2D currentLevel = levels[levelNumber];

            //go through all the y positions of our image, the rows
            for (int y = 0; y < currentLevel.height; y++)
            {
                //go through all the x positions of our image, the columns
                for (int x = 0; x < currentLevel.width; x++)
                {
                    //get the current pixel value at that coordinate
                    Color pixel = currentLevel.GetPixel(x, y);

                    //If the pixel matches the color we set for our player. . .
                    if (pixel == playerColor)
                    {
                        //Create player in our scene and store it in a variable
                        myPlayer = Instantiate(playerPrefab,
                                      new Vector3(x, y, 0f), Quaternion.identity);
                    }
                    else if (pixel == enemyColor)
                    {
                        //choose a random enemy from an array of enemy prefabs
                        enemyList.Add(Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], new Vector3(x, y, 0f), Quaternion.identity));
                    }
                  //wall colors are separated, optional to do the same for enemies
                    else if (pixel == blueWallColor)
                    {
                        //add the wall GameObjects to the wall list
                        wallList.Add(Instantiate(blueWallPrefab,
                                     new Vector3(x, y, 0f), Quaternion.identity));
                    }
                    else if (pixel == greenWallColor)
                    {
                        wallList.Add(Instantiate(greenWallPrefab,
                                     new Vector3(x, y, 0f), Quaternion.identity));
                    }
                    else if (pixel == orangeWallColor)
                    {
                        wallList.Add(Instantiate(orangeWallPrefab,
                                     new Vector3(x, y, 0f), Quaternion.identity));
                    }
                    else if (pixel == weaponColor)
                    {
                        Instantiate(
                           weaponPrefabs[Random.Range(0, weaponPrefabs.Length)],
                           new Vector3(x, y, 0f), Quaternion.identity);
                    }
                }
            }
        }
        weaponList.AddRange(GameObject.FindGameObjectsWithTag("Weapon"));
        //ensure the camera follows the newly created player
        levelCamera.Follow = myPlayer.transform;
        levelCamera.LookAt = myPlayer.transform;
    }

    //next level deactivates all screen except the game screen and increases level
    public void NextLevel()
    {
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        gameScreen.SetActive(true);
        if(myPlayer && enemyList.Count == 0)
        {
            //increase level number
            levelNumber++;
            //remainder keeps the levelNumber between 0 and the total levels
            levelNumber %= levels.Length;
            //create another new level!
            CreateLevel(levelNumber);
        }
    }

}
