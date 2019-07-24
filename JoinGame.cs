using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGame : MonoBehaviour
{
    //player selected Game
    public string gameID = "Game_01";


    //waiting in the lobby for another player 
    public string playerID = "abc118";
    public static string searchingChecker = "true";
    public static string InGameChecker = "false";

    //opponents game state
    public string opponentSearchingChecker;
    public string opponentInGameChecker;



    public List<string> playerIDList = new List<string>();
    public List<string> opponentUserId = new List<string>();

    private void Awake()
    {   //Calling firebase 
        InitializeFireBase();
    }


    private void Start()
    {
        print("start");
        MatchmakerCallbacks();

    }





    void InitializeFireBase()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://matchmaker-7c2f1.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }



    //Controls where player should be put Lobby or InGame
    public void MatchCondition()
    {
        if (string.Equals(searchingChecker, "true") && string.Equals(InGameChecker, "false"))
        {
            Debug.Log("Lobby");
        }
        else if (string.Equals(searchingChecker, "false") && string.Equals(InGameChecker, "true"))
        {
            print("In Game");
        }

    }


    public void Lobby()
    {
        int n = Random.RandomRange(1, 500);
        //When player is ready for matchmaking. 
        searchingChecker = "true";
        InGameChecker = "false";

        //Database Push(). Update Player state. 
        User user = new User();

        RestClient.Put("https://matchmaker-7c2f1.firebaseio.com/" + gameID + "/Lobby/" + playerID + n + ".json", user); //Game_01>>>Lobby>>>userID:abc123>>>player condition

        //   MatchmakerCallbacks(); //Matchmaking process starts


    }
    //Fetches list of the player from lobby by their userID 
    public void PlayerListCollector()
    {

        Firebase.Database.FirebaseDatabase dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
        dbInstance.GetReference(gameID + "/Lobby").GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  foreach (DataSnapshot user in snapshot.Children)
                  {
                      IDictionary dictUser = (IDictionary)user.Value;

                      playerIDList.Add(dictUser["userId"].ToString());


                  }
              }
          });

    }
    //starts the matchmaking process among the users 
    public void MatchMaker()
    {


        //Then check if the user is not itself then search for users game status.
        foreach (string opponenetID in playerIDList)
        {
            bool flag = false;
            if (playerID != opponenetID)
            {
                RestClient.Get<User>("https://matchmaker-7c2f1.firebaseio.com/" + gameID + "/Lobby/" + opponenetID + ".json").Then(response =>
                {
                    //Gets the opponent data
                    opponentSearchingChecker = response.userSearching;
                    opponentInGameChecker = response.userInGame;
                    if (string.Equals(opponentSearchingChecker, "true") && string.Equals(opponentInGameChecker, "false"))
                    {
                        //Available players in the lobby
                        
                        opponentUserId.Add(opponenetID);


                    }
                    else
                    {

                        //  print(opponentSearchingChecker + "  -   " + opponentInGameChecker);
                    }



                });



            }
            else //This is user himself
            {
                print("Me matched myself");

            }

        }
    }

    //First collects the id's from firebase then Invokes for Matchmaking after a period of Time
    public void MatchmakerCallbacks()
    {
        PlayerListCollector();
        Invoke("MatchMaker", 3f);
        Invoke("InGame", 7f);

    }




    public void InGame()
    {
        print(opponentUserId[0]);

        PostToDatabase(opponentUserId[0]);
        PostToDatabase(playerID);

    }

    void PostToDatabase(string id)
    {
        //Database Push(). Update Player state. 
        User user = new User();
        user.userInGame = "true";
        user.userSearching = "false";
        user.userId = id;


        RestClient.Put("https://matchmaker-7c2f1.firebaseio.com/" + gameID + "/In_Game/" + id + ".json", user); //Game_01>>>In_Game>>>userID:abc123>>>player condition

        //   MatchmakerCallbacks(); //Matchmaking process starts
    }


    public void GetFromDatabase(string player_ID)
    {

        RestClient.Get<User>("https://matchmaker-7c2f1.firebaseio.com/" + gameID + "/Lobby/" + player_ID + ".json").Then(response =>
           {
               //Gets the opponent data
               opponentSearchingChecker = response.userSearching;
               opponentInGameChecker = response.userInGame;



           });

    }


}
