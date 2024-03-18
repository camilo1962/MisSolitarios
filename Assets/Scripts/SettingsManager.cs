using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public static UnityAction RESET;
    public delegate bool UNDOFUNC();
    public static UNDOFUNC UNDO;
    const string KLONDIKE_RULES = "Para ganar un juego de Clásico, uno debe mover todas las cartas al área de los cuatro ases. Estas cartas deben ser de palos iguales y van del As al Rey para los 4 palos.",
        PYRAMID_RULES = "Para ganar un juego de Pirámide, uno debe quitar todas las cartas del tablero. La cubierta no necesita ser limpiada también. Para borrar una tarjeta, se debe hacer una coincidencia igual a 13. Un rey se elimina solo.",
        TRI_PEAKS_RULES = "Para ganar un juego de Tres Pilas, uno debe quitar todas las cartas del tablero. La cubierta no necesita ser limpiada también. Para borrar una tarjeta, se debe hacer clic en una tarjeta que tenga un valor por encima o por debajo de la tarjeta actual. Si no hay más coincidencias, entonces uno puede robar una nueva carta o reiniciar el juego.",
        FREECELL_RULES = "Para ganar un juego de Celda Libre, uno debe mover todas las cartas a la ubicación superior derecha. También conocida como la ubicación de Aces. Solo se puede mover una carta a la vez. Pero el juego permitirá mover varias cartas si hay espacio libre para moverlas.",
        SPIDER_RULES = "Para ganar un juego de Araña Solitario, uno debe completar carreras desde el rey hasta el as hasta que todas las cartas se eliminen del área de juego. No es necesario sacar las cartas para ganar, pero eso puede ayudar a continuar el juego.",
        BLACK_WAR_RULES = "Para ganar un juego de Guerra Negra, ​​uno debe ganar más cartas que cualquier otro jugador. Actualmente BlackWar está en modo Beta.",
        SOUND_IS_ACTIVE_PREFS = "SoundIsActive",
        MUSIC_IS_ACTIVE_PREFS = "MusicIsActive",
        LIMIT_DRAW_PREFS = "limitDraw",
        DRAW_THREE_PREFS = "drawThree",
        TIME_FORMAT_PREFS = "formatTime",
        COUNT_UNDO_PREFS = "countUndo",
        LIMIT_UNDO_PREFS = "limitUndo";

    [Header("Game Settings")]
    [SerializeField] const int UNDO_LIMIT_MAX = 5,
        DRAW_LIMIT_MAX = 2;
    [SerializeField] int currentUndosLeft;
    [SerializeField] int currentDeckRefreshesLeft;

    public static bool hasSound;
    public static bool hasMusic;

    [Header("Si el usuario puede jugar o no. es falso cuando el menú está abierto o hasWon es verdadero")]
    [SerializeField] bool isGameActive;

    [Header("Si el usuario puede mover una carta o no. Es falso mientras se mueve una carta.")]
    [SerializeField] bool canMoveCard;

    #region Settings
    [Header("Settings Screens")]
    [SerializeField] GameObject mainSettings;
    [SerializeField] GameObject advancedOptions;

    [Header("Notify and Verify")]
    [SerializeField] GameObject notificationScreen;
    [SerializeField] Text notificationText;
    [SerializeField] GameObject verificationScreen;
    [SerializeField] Text verificationQuestionText;
    [SerializeField] Text verificationConfirmButtonText;
    [SerializeField] Text verificationCancelButtonText;
    [SerializeField] Button verificationConfirmButton;
    [SerializeField] Button verificationCancelButton;

    [Header("Ajuste Botones")]
    [SerializeField] GameObject extraSettingsButtonTimeFormat;
    [SerializeField] GameObject[] extraSettingsButtons;

    [Header("Reglas")]
    [SerializeField] Text rulesText;
    #endregion Settings

    #region audio
    [Header("Música del Juego")]
    [SerializeField] AudioSource gameMusic;
    [SerializeField] AudioSource cheersSound;
    [SerializeField] AudioSource clickSound;
    [SerializeField] AudioSource victorySound;
    [SerializeField] AudioSource dealSound;
    [SerializeField] AudioSource flipSound;

    [Header("Las imágenes de los botones para el sonido, la música y el tiempo.")]
    [SerializeField] Image soundButtonImage;
    [SerializeField] Image musicButtonImage;
    Sprite soundOnSprite,
        soundOffSprite,
        musicOnSprite,
        musicOffSprite;
    #endregion audio

    [Header("The prefab for the risingScore")]
    [SerializeField] GameObject risingScorePrefab;
    private List<RisingScore> risingObjects = new List<RisingScore>();

    #region victory screen variables
    [Header("La pantalla de la victoria")]
    [SerializeField] GameObject victoryScreen;

    [Header("The victory screen texts")]
    [SerializeField] Text leftVictoryText;
    [SerializeField] Text rightScoreVictoryText;
    [SerializeField] Text gameScoreVictoryText;
    #endregion victory screen variables

    #region switches
    [Header("Switches")]
    [SerializeField] bool isDraw3;
    [SerializeField] Image drawSwitch;
    [SerializeField] bool isLimitDeckRefresh;
    [SerializeField] Image limitDeckSwitch;
    [SerializeField] bool isTimeFormat;
    [SerializeField] Image timeFormatSwitch;
    [SerializeField] bool isCountUndo;
    [SerializeField] Image countUndoSwitch;
    [SerializeField] bool isLimitUndo;
    [SerializeField] Image limitUndoSwitch;
    #endregion switches

    #region BlackWar Variables
    [SerializeField] GameObject blackWarSettings;
    #endregion BlackWar Variables

    bool isResetOnAdvancedClose = false;

    public static int activeBack=0;
    public Sprite[] backs;
    public static int activeDeck = 0;
    [SerializeField] DeckFront[] deckFronts;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject); // On reload, singleton already set, so destroy duplicate.
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        SetVariables();
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);

        //if (hasMusic && !gameMusic.isPlaying) gameMusic.Play();
    }

    public Sprite GetCardBack()
    {
        return backs[activeBack];
    }

    public DeckFront GetDeckFront()
    {
        return deckFronts[activeDeck];
    }

    public Sprite GetCardFront(int suit, int number)
    {
        return deckFronts[activeDeck].deck[suit].cards[number];
    }

    public Sprite[] GetFrontAces()
    {
        List<Sprite> fronts = new List<Sprite>();

        foreach(DeckFront deckFront in deckFronts)
        {
            fronts.Add(deckFront.deck[0].cards[0]);
        }

        return fronts.ToArray();
    }

    public void CloseAdvancedSettings()
    {
        if (isResetOnAdvancedClose)
        {
            ResetGame();
            isResetOnAdvancedClose = false;
        }

        advancedOptions.SetActive(false);
    }

    /**
     * Sets the variables and switches for the game
     */
    private void SetVariables()
    {
        currentUndosLeft = UNDO_LIMIT_MAX;
        currentDeckRefreshesLeft = DRAW_LIMIT_MAX;

        isGameActive = true;
        canMoveCard = true;

        #region switches
        //Handle Switches
        isLimitDeckRefresh = PlayerPrefs.GetInt(LIMIT_DRAW_PREFS, 0) == 1;
        isDraw3 = PlayerPrefs.GetInt(DRAW_THREE_PREFS, 0) == 1;
        isTimeFormat = PlayerPrefs.GetInt(TIME_FORMAT_PREFS, 0) == 1;
        isCountUndo = PlayerPrefs.GetInt(COUNT_UNDO_PREFS, 0) == 1;
        isLimitUndo = PlayerPrefs.GetInt(LIMIT_UNDO_PREFS, 0) == 1;

        if (isLimitDeckRefresh)
        {
            limitDeckSwitch.transform.localPosition = new Vector3(100, 0);
            limitDeckSwitch.color = Color.green;
        }
        else
        {
            limitDeckSwitch.transform.localPosition = new Vector3(-100, 0);
            limitDeckSwitch.color = Color.red;
        }

        if (isDraw3)
        {
            drawSwitch.transform.localPosition = new Vector3(100, 0);
            drawSwitch.color = Color.green;
        }
        else
        {
            drawSwitch.transform.localPosition = new Vector3(-100, 0);
            drawSwitch.color = Color.red;
        }

        if (isTimeFormat)
        {
            timeFormatSwitch.transform.localPosition = new Vector3(100, 0);
            timeFormatSwitch.color = Color.green;
        }
        else
        {
            timeFormatSwitch.transform.localPosition = new Vector3(-100, 0);
            timeFormatSwitch.color = Color.red;
        }

        if (isCountUndo)
        {
            countUndoSwitch.transform.localPosition = new Vector3(100, 0);
            countUndoSwitch.color = Color.green;
        }
        else
        {
            countUndoSwitch.transform.localPosition = new Vector3(-100, 0);
            countUndoSwitch.color = Color.red;
        }

        if (isLimitUndo)
        {
            limitUndoSwitch.transform.localPosition = new Vector3(100, 0);
            limitUndoSwitch.color = Color.green;
        }
        else
        {
            limitUndoSwitch.transform.localPosition = new Vector3(-100, 0);
            limitUndoSwitch.color = Color.red;
        }
        #endregion switches

        #region Handle Sound
        hasSound = PlayerPrefs.GetInt(SOUND_IS_ACTIVE_PREFS, 1) == 1;
        hasMusic = PlayerPrefs.GetInt(MUSIC_IS_ACTIVE_PREFS, 1) == 1;
        soundOnSprite = Resources.Load<Sprite>("Images/Icons/Sound");
        soundOffSprite = Resources.Load<Sprite>("Images/Icons/NoSound");
        musicOnSprite = Resources.Load<Sprite>("Images/Icons/Music");
        musicOffSprite = Resources.Load<Sprite>("Images/Icons/NoMusic");
        soundButtonImage.sprite = hasSound ? soundOnSprite : soundOffSprite;
        musicButtonImage.sprite = hasMusic ? musicOnSprite : musicOffSprite;
        #endregion Handle Sound

        victoryScreen.SetActive(false);
        mainSettings.SetActive(false);
    }

    /**
     * Call this before resetting deck.
     */
    public bool CanRefreshDeck(Text deckRefreshText)
    {
        if (!isLimitDeckRefresh || (isLimitDeckRefresh && (currentDeckRefreshesLeft < 1))) return false;

        currentDeckRefreshesLeft--;

        if (deckRefreshText != null) deckRefreshText.text = isLimitDeckRefresh? "" + currentDeckRefreshesLeft :"";

        return true;
    }

    /**
     * gets deck refresh
     */
    public bool IsLimitDeckRefresh()
    {
        return isLimitDeckRefresh;
    }

    /**
     * Gets settings is active
     */
    public bool IsSettingsOpen()
    {
        return mainSettings.activeInHierarchy;
    }

    /**
     * Gets itCountUndo
     */
    public bool IsCountUndo()
    {
        return isCountUndo;
    }

    /**
     * Gets isLimitUndo
     */
    public bool IsLimitUndo()
    {
        return isLimitUndo;
    }

    /**
     * returns isDrawThree
     */
    public bool IsDrawThree()
    {
        return isDraw3;
    }

    /**
     * gets isGameActive
     */
    public bool IsGameActive()
    {
        return isGameActive;
    }

    /**
     * Gets canMoveCard
     */
    public bool CanMoveCard()
    {
        return canMoveCard;
    }

    /**
     * Sets isGameActive
     */
    public void SetIsGameActive(bool isGameActive)
    {
        this.isGameActive = isGameActive;
    }

    /**
     * Getter for deck refreshes left
     */
    public int GetDeckRefreshesLeft()
    {
        return currentDeckRefreshesLeft;
    }

    /**
     * Gets currentUndosLeft
     */
    public int GetCurrentUndosLeft()
    {
        return currentUndosLeft;
    }

    /**
     * Undoes the refresh deck
     */
    public void UndoRefreshDeck(Text deckRefreshText)
    {
        if (!isLimitDeckRefresh) return;
        currentDeckRefreshesLeft++;
        if (deckRefreshText != null) deckRefreshText.text = "" + currentDeckRefreshesLeft;
    }

    /**
     * Formats the time to be displayed
     */
    public string FormatTime(float time)
    {
        string formattedTime;

        if (isTimeFormat)
        {
            int milliseconds = (int)(((time % 1) * 1000) % 1000);

            int seconds = (int)time;//(int)time;
            int minutes = 0;
            int hours = 0;
            int days = 0;

            //seconds now
            if (seconds > 59)
            {
                //get minutes
                minutes = seconds / 60;
                seconds %= 60;

                if (minutes > 59)
                {
                    //get hours
                    hours = minutes / 60;
                    minutes %= 60;

                    if (hours > 23)
                    {
                        //get days
                        days = hours / 24;
                        hours %= 24;
                    }
                }
            }

            formattedTime = ""
                + (days > 0 ? days + ":" : "")
                + (hours > 0 ? hours + ":" : "")
                + (minutes > 0 ? minutes + ":" : "")
                + (seconds > 0 ? seconds + ":" : "")
                + (milliseconds > 0 ? milliseconds + "" : "0");
        }
        else
        {
            formattedTime = "" + (int)time;
        }

        return formattedTime;
    }

    /**
     * Closes the application or minimizes on Android
     */
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ViewCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ChangeCards()
    {
        SceneManager.LoadScene("ChangeCards");
    }

    /**
     * Attempts to load the scene with sceneName
     */
    public void LoadScene(string sceneName)
    {
        extraSettingsButtons[0].SetActive(false);
        extraSettingsButtons[1].SetActive(false);

        switch (sceneName)
        {
            case "Klondike":
                #region Klondike
                extraSettingsButtonTimeFormat.SetActive(true);
                extraSettingsButtons[0].SetActive(true);
                extraSettingsButtons[1].SetActive(true);

                //countUndo moves
                extraSettingsButtons[2].SetActive(true);

                //limitUndo
                extraSettingsButtons[3].SetActive(true);

                rulesText.text = KLONDIKE_RULES;
                break;
                #endregion Klondike
            case "Pyramid":
                #region Pyramid
                extraSettingsButtonTimeFormat.SetActive(true);

                //countUndo moves
                extraSettingsButtons[2].SetActive(false);

                //limitUndo
                extraSettingsButtons[3].SetActive(true);

                rulesText.text = PYRAMID_RULES;
                break;
                #endregion Pyramid
            case "TriPeaks":
                #region TriPeaks
                extraSettingsButtonTimeFormat.SetActive(false);

                //countUndo moves
                extraSettingsButtons[2].SetActive(false);

                //limitUndo
                extraSettingsButtons[3].SetActive(true);

                rulesText.text = TRI_PEAKS_RULES;
                break;
                #endregion TriPeaks
            case "FreeCell":
                #region freeCell
                extraSettingsButtonTimeFormat.SetActive(true);

                //countUndo moves
                extraSettingsButtons[2].SetActive(true);

                //limitUndo
                extraSettingsButtons[3].SetActive(true);

                rulesText.text = FREECELL_RULES;
                break;
                #endregion freeCell
            case "Spider":
                #region spider
                extraSettingsButtonTimeFormat.SetActive(true);

                //countUndo moves
                extraSettingsButtons[2].SetActive(true);

                //limitUndo
                extraSettingsButtons[3].SetActive(true);

                rulesText.text = SPIDER_RULES;
                break;
                #endregion spider
            case "BlackWar":
                #region blackWar
                extraSettingsButtonTimeFormat.SetActive(true);

                //countUndo moves
                extraSettingsButtons[2].SetActive(false);

                //limitUndo
                extraSettingsButtons[3].SetActive(false);

                rulesText.text = BLACK_WAR_RULES;
                break;
                #endregion blackWar
        }

        SceneManager.LoadScene(sceneName);
    }

    /**
     * verifies where the user wants to quit to.
     */
    public void VerifyQuitGame()
    {
        //create notification asking for user input
        SetVerification(false, "¿Volver a la pantalla principal o al escritorio?", "Pantalla principal", "Escritorio", Return, QuitGame);

        void Return()
        {
            LoadScene("MainMenuScene");
        }
    }

    /**
     * Resets the game
     */
    public void ResetGame()
    {
        SetVariables();

        RESET();
    }

    /**
     * Runs a controllers undo function
     */
    public void UndoMove()
    {
        if (isLimitUndo && (currentUndosLeft < 1)) return;

        if (isLimitUndo)
        {
            currentUndosLeft--;
        }

        if (!UNDO() && isLimitUndo)
        {       
            currentUndosLeft++;
        }
    }

    /**
     * Turns on and off the sound
     */
    public void ToggleSound()
    {
        //turn on or off the sound
        if (hasSound)
        {
            hasSound = false;
            cheersSound.Stop();
            clickSound.Stop();
            victorySound.Stop();
        }
        else
        {
            hasSound = true;
        }

        //save in prefs
        PlayerPrefs.SetInt(SOUND_IS_ACTIVE_PREFS, hasSound ? 1 : 0);

        //set the image
        soundButtonImage.sprite = hasSound ? soundOnSprite : soundOffSprite;
    }

    /**
     * Turns on and off the music
     */
    public void ToggleMusic()
    {
        hasMusic = !hasMusic;

        if (hasMusic)
        {
            //gameMusic.Play();
        }
        else
        {
            gameMusic.Stop();
        }

        //save in prefs
        PlayerPrefs.SetInt(MUSIC_IS_ACTIVE_PREFS, hasMusic ? 1 : 0);

        //set the image
        musicButtonImage.sprite = hasMusic ? musicOnSprite : musicOffSprite;
    }

    /**
     * Turns the settings on and off
     */
    public void ToggleSettings()
    {
        mainSettings.SetActive(!mainSettings.activeInHierarchy);

        //see if we have a timer
        StopWatch stopWatch = FindObjectOfType<StopWatch>();

        if(stopWatch != null)
        {
            //see if we should play/pause the timer
            if (mainSettings.activeInHierarchy)
            {
                stopWatch.PauseStopWatch();
            }
            else
            {
                stopWatch.ResumeStopWatch();
            }
        }
    }

    /**
     * Turns on and off the advanced settings
     */
    public void ToggleAdvancedSettings()
    {
        advancedOptions.SetActive(!advancedOptions.activeInHierarchy);
    }

    /**
     * Creates a notification
     */
    public void SetNotification(string notification)
    {
        notificationText.text = notification;
        notificationScreen.SetActive(true);
    }

    /**
     * Closes the notification screen.
     */
    public void CloseNotify()
    {
        notificationScreen.SetActive(false);
    }

    /**
     * Creates a verification message for the user
     */
    public void SetVerification(bool closeSettings, string question, string onAccept, string onDeny, UnityAction acceptAction, UnityAction denyAction)
    {
        verificationScreen.SetActive(true);
        verificationQuestionText.text = question;
        verificationConfirmButtonText.text = onAccept;
        verificationCancelButtonText.text = onDeny;

        verificationConfirmButton.onClick.AddListener(AcceptAction);
        verificationCancelButton.onClick.AddListener(DenyAction);

        void AcceptAction()
        {
            acceptAction();
            verificationScreen.SetActive(false);
            if(closeSettings) mainSettings.SetActive(false);
            ClearActions();
        }

        void DenyAction()
        {
            if(denyAction!=null) denyAction();
            verificationScreen.SetActive(false);
            if (closeSettings) mainSettings.SetActive(false);
            ClearActions();
        }

        void ClearActions()
        {
            verificationConfirmButton.onClick.RemoveAllListeners();
            verificationCancelButton.onClick.RemoveAllListeners();
        }
    }

    /**
     * Cancels the verification without following either button
     */
    public void CancelVerify()
    {
        verificationScreen.SetActive(false);
    }

    /**
     * Creates a rising score
     */
    public void ShowScore(int score, Transform instTransform)
    {
        PlayCheersSound();

        //see if we have a risingObject that is not active
        foreach (RisingScore rising in risingObjects)
        {
            if (!rising.isActive)
            {
                rising.StartRising(score);
                return;
            }
        }

        //instantiate a new floatingScore object
        GameObject instantiatedGameObject = Instantiate(risingScorePrefab, instTransform);

        RisingScore risingScore = instantiatedGameObject.GetComponent<RisingScore>();
        risingObjects.Add(risingScore);

        if (risingScore != null)
        {
            risingScore.StartRising(score);
        }
    }

    /**
     * Toggles Klondike's Draw 3 and Draw 1
     */
    public void SwitchDraw3_1()
    {
        SetVerification(false, "Alternar este interruptor reiniciará el juego.", "Continua", "Cancela", ToggleSwitch, null);

        void ToggleSwitch()
        {
            isDraw3 = !isDraw3;

            PlayerPrefs.SetInt(DRAW_THREE_PREFS, isDraw3 ? 1 : 0);

            if (isDraw3)
            {
                drawSwitch.transform.localPosition = new Vector3(100, 0);
                drawSwitch.color = Color.green;
            }
            else
            {
                drawSwitch.transform.localPosition = new Vector3(-100, 0);
                drawSwitch.color = Color.red;
            }

            isResetOnAdvancedClose = true;
        }
    }

    /**
     * Toggles deck cycles:
     *      true: 3
     *      false: unlimited
     */
    public void SwitchLimitDeckCycles()
    {
        SetVerification(false, "Alternar este interruptor reiniciará el juego.", "Continua", "Cancela", ToggleSwitch, null);

        void ToggleSwitch()
        {
            isLimitDeckRefresh = !isLimitDeckRefresh;

            PlayerPrefs.SetInt(LIMIT_DRAW_PREFS, isLimitDeckRefresh ? 1 : 0);

            if (isLimitDeckRefresh)
            {
                limitDeckSwitch.transform.localPosition = new Vector3(100, 0);
                limitDeckSwitch.color = Color.green;
            }
            else
            {
                limitDeckSwitch.transform.localPosition = new Vector3(-100, 0);
                limitDeckSwitch.color = Color.red;
            }

            isResetOnAdvancedClose = true;
        }
    }

    /**
     * Toggles the time format:
     *      true: 12:59:123
     *      false: 254 seconds
     */
    public void SwitchTimeFormat()
    {
        isTimeFormat = !isTimeFormat;

        PlayerPrefs.SetInt(TIME_FORMAT_PREFS, isTimeFormat ? 1 : 0);

        if (isTimeFormat)
        {
            timeFormatSwitch.transform.localPosition = new Vector3(100, 0);
            timeFormatSwitch.color = Color.green;
        }
        else
        {
            timeFormatSwitch.transform.localPosition = new Vector3(-100, 0);
            timeFormatSwitch.color = Color.red;
        }
    }

    /**
     * Toggles countUndo
     *      true: undo is counted as a move
     *      false: undo is not counted as a move
     */
    public void SwitchCountUndo()
    {
        SetVerification(false, "Alternar este interruptor reiniciará el juego.", "Continua", "Cancela", ToggleSwitch, null);

        void ToggleSwitch()
        {
            isCountUndo = !isCountUndo;

            PlayerPrefs.SetInt(COUNT_UNDO_PREFS, isCountUndo ? 1 : 0);

            if (isCountUndo)
            {
                countUndoSwitch.transform.localPosition = new Vector3(100, 0);
                countUndoSwitch.color = Color.green;
            }
            else
            {
                countUndoSwitch.transform.localPosition = new Vector3(-100, 0);
                countUndoSwitch.color = Color.red;
            }

            isResetOnAdvancedClose = true;
        }
    }

    /**
     * Limits the number of undo's that a user can use
     *      true: 5 undos
     *      false: unlimited
     */
    public void SwitchLimitUndo()
    {
        SetVerification(false, "Alternar este interruptor reiniciará el juego.", "Continua", "Cancela", ToggleSwitch, null);

        void ToggleSwitch()
        {
            isLimitUndo = !isLimitUndo;

            PlayerPrefs.SetInt(LIMIT_UNDO_PREFS, isLimitUndo ? 1 : 0);

            if (isLimitUndo)
            {
                limitUndoSwitch.transform.localPosition = new Vector3(100, 0);
                limitUndoSwitch.color = Color.green;
            }
            else
            {
                limitUndoSwitch.transform.localPosition = new Vector3(-100, 0);
                limitUndoSwitch.color = Color.red;
            }

            isResetOnAdvancedClose = true;
        }
    }

    /**
     * Sets the victory screen
     */
    public void SetVictory(string gameScore, string leftText, string rightText)
    {
        victoryScreen.SetActive(true);

        gameScoreVictoryText.text = gameScore;
        leftVictoryText.text = leftText;
        rightScoreVictoryText.text = rightText;

        PlayVictoryCheerSound();
    }

    #region Sound Section
    public void PlayDealSound()
    {
        if (hasSound)
        {
            //dealSound.Play();
        }
    }

    public void PlayVictoryCheerSound()
    {
        if (hasSound)
        {
            //victorySound.Play();
        }
    }

    public void PlayClickSound()
    {
        if (hasSound)
        {
            //clickSound.Play();
        }
    }

    public void PlayFlipSound()
    {
        if (hasSound)
        {
            //flipSound.Play();
        }
    }

    public void PlayCheersSound()
    {
        if (hasSound)
        {
            //cheersSound.Play();
        }
    }
    #endregion Sound Section

    #region BlackWarStarter
    public void ToggleBlackWarOpener()
    {
        blackWarSettings.SetActive(!blackWarSettings.activeInHierarchy);
    }

    public void BlackWarButton(int i)
    {
        BlackWarController.numberOfPlayers = i+2;
        LoadScene("BlackWar");
    }
    public  void Salir()
    {
        Application.Quit();
    }
    #endregion BlackWarStarter
}
