/*                                                                                     *\
           _____ _____   _____   _      _              _____  _     _           
          / ____|  __ \ / ____| | |    (_)            |  __ \(_)   | |          
         | |  __| |  | | (___   | |     _ _ __   ___  | |__) |_  __| | ___ _ __ 
         | | |_ | |  | |\___ \  | |    | | '_ \ / _ \ |  _  /| |/ _` |/ _ \ '__|
         | |__| | |__| |____) | | |____| | | | |  __/ | | \ \| | (_| |  __/ |   
          \_____|_____/|_____/  |______|_|_| |_|\___| |_|  \_\_|\__,_|\___|_|   
      ˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽
        ©2016 Sionera Entertainment - Daniele Banovaz (daniele.banovaz@gmail.com)

        Developed as a tutorial for Game Developement Saturday #1, 2016-03-19, PN

\*                                                                                     */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager class that handles racing phase
/// </summary>
public class RaceManager : MonoBehaviour
{
    /// <summary>
    /// Link to Track designer
    /// </summary>
    public TrackDesigner TrackDesigner;

    /// <summary>
    /// Link to the Car
    /// </summary>
    public Car Car;


    #region Coins

    /// <summary>
    /// Sound source player when a Coin is collected
    /// </summary>
    public AudioSource CoinCollectSound;

    /// <summary>
    /// Link to Coin template, used to spawn random coins at the beginning of the match
    /// </summary>
    public GameObject CoinPrefab;
    
    /// <summary>
    /// Amount of Coins to spawn
    /// </summary>
    public int CoinsToSpawn = 20;

    /// <summary>
    /// Amount of randomness in Coin positions
    /// </summary>
    public float MaxCoinPositionRandomness = 6;

    /// <summary>
    /// Main direction to follow when generating coins
    /// </summary>
    public Vector2 MainCoinGenerationDirection = new Vector2(5, -8);

    /// <summary>
    /// Reference to coins present in the Scene
    /// </summary>
    private List<Coin> _coins;

    /// <summary>
    /// Current Score
    /// </summary>
    public int Score
    {
        get { return _score; }
        private set
        {
            _score = value;

            // Update score text
            ScoreText.text = _score.ToString();

            // If new high score has been reached..
            if (_score > PlayerPrefs.GetInt("Record"))
            {
                // ..persist it into PlayerPrefs..
                PlayerPrefs.SetInt("Record", _score);

                // ..and update record text
                PreviousRecordText.text = "NEW RECORD: " + _score;
                PreviousRecordText.color = new Color(1, 0.3f, 0f);
            }
        }
    }

    /// <summary>
    /// Backing field for Score property
    /// </summary>
    private int _score;

    #endregion Coins


    #region UI

    /// <summary>
    /// Link to the panel that contains Designer's UI elements
    /// </summary>
    public GameObject DesignerUI;

    /// <summary>
    /// Link to the panel that contains Racing phase's UI elements
    /// </summary>
    public GameObject RunUI;

    /// <summary>
    /// Text used to display current score of the Player
    /// </summary>
    public Text ScoreText;

    /// <summary>
    /// Text used to display current record
    /// </summary>
    public Text PreviousRecordText;

    #endregion UI

    /// <summary>
    /// Determine when to stop falling
    /// </summary>
    private float _minHeight;


    /// <summary>
    /// Called by Unity when this Component is created
    /// </summary>
    private void Awake()
    {
        // Get reference to existing Coins, if any
        _coins = new List<Coin>(FindObjectsOfType<Coin>());

        // Generate random coins as required
        GenerateRandomCoins(CoinsToSpawn - _coins.Count, Car.transform.position, MainCoinGenerationDirection);

        // Link Coin collection event
        foreach (Coin coin in _coins)
            coin.Collected += HandleCollectedCoin;

        // Set previous record text, reading from stored PlayerPrefs
        PreviousRecordText.text = "Previous record: " + PlayerPrefs.GetInt("Record");
    }

    /// <summary>
    /// Switch to Race phase
    /// </summary>
    public void StartRace()
    {
        if (Car.IsRunning)
            return;

        // Switch to Race's UI
        TrackDesigner.enabled = false;
        DesignerUI.SetActive(false);
        RunUI.SetActive(true);

        // Remove faded out appearance from collected Coins
        foreach (Coin coin in _coins)
            coin.ResetColor();

        // Calculate new min height
        _minHeight = TrackDesigner.CalculateMinHeight() - 50;

        // Start the car
        Car.IsRunning = true;
    }

    /// <summary>
    /// Stop the Race and switch to Designer
    /// </summary>
    public void StopRace()
    {
        if (!Car.IsRunning)
            return;
        
        // Stop the car
        Car.IsRunning = false;

        // Switch to Designer's UI
        TrackDesigner.enabled = true;
        DesignerUI.SetActive(true);
        RunUI.SetActive(false);

        // Re-enable collected coins
        foreach (Coin coin in _coins)
            coin.gameObject.SetActive(true);

        // Reset score
        Score = 0;
    }

    /// <summary>
    /// Handle collision between Car and Coin
    /// </summary>
    /// <param name="collectedCoin"></param>
    public void HandleCollectedCoin(Coin collectedCoin)
    {
        // Play "Coin Collected" sound
        CoinCollectSound.Play();

        // Add its value to score (5 points if it's a "MegaCoin", 1 otherwise)
        Score += collectedCoin.MegaCoin ? 5 : 1;
    }

    /// <summary>
    /// Generate random coins to be chased by the running Car
    /// </summary>
    /// <param name="amount">Total number of Coins to spawn</param>
    /// <param name="startingPosition">Initial position to start from</param>
    /// <param name="direction">Direction to follow</param>
    public void GenerateRandomCoins(int amount, Vector2 startingPosition, Vector2 direction)
    {
        for (int i = 0; i < amount; i++)
        {
            // Get next slightly randomized position
            startingPosition += direction + new Vector2(Random.value, Random.value) * MaxCoinPositionRandomness;

            // Create new GameObject in the specified poistion (and set it as a child of this one)
            GameObject newCoinGO = GameObject.Instantiate(CoinPrefab, startingPosition, Quaternion.identity) as GameObject;
            newCoinGO.transform.parent = transform;

            // Sometimes, spawn a MegaCoin
            Coin newCoin = newCoinGO.GetComponent<Coin>();
            if ((Random.value > 0.9f)||(i == amount - 1))
                newCoin.MegaCoin = true;

            // Keep track of existing Coins
            _coins.Add(newCoin);
        }
    }

    /// <summary>
    /// Reload the Scene to allow a new Coin Randomization
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    /// <summary>
    /// Called by Unity engine once per frame
    /// </summary>
    private void Update()
    {
        if (!Car.IsRunning)
            return;

        // If the Car is falling below minimum terrain height, reset
        if (Car.CarBody.position.y < _minHeight)
            StopRace();
    }
}
