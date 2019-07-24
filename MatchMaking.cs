using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class MatchMaking : MonoBehaviour
{


    List<string> playerIdList = new List<string>();

    public static int num = 100;
    private string firebaseURL = "https://matchmaker-7c2f1.firebaseio.com/";



    User user = new User();


    private void Awake()
    {
        InitializeFirebase();
     
        
  
    }

    private void Start()
    {
        GetDataList();
        
        // RestClient.Delete("https://matchmaker-7c2f1.firebaseio.com/" + "Game_01/" + ".json");
    }

    private void Update()
    {
         Invoke("PostToDatabase",2f);
     

    }




    void InitializeFirebase()
    {

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://matchmaker-7c2f1.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

    }



    void PostToDatabase()
    {

        User user = new User();
        num++;
        RestClient.Put("https://matchmaker-7c2f1.firebaseio.com/" + "Game_01/" + num.ToString() + ".json", user);

    }


    void GetFromDatabase()
    {

        RestClient.Get<User>("https://matchmaker-7c2f1.firebaseio.com/" + "Game_01/" + ".json").Then(response =>
         {
             user = response;
             print("user" + response.userId);

         });
    }




    void GetDataList() //Collects the player information from the tree
    {


        Firebase.Database.FirebaseDatabase dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
        dbInstance.GetReference("Game_01").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                print("No data");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children)
                {
                    IDictionary dictUser = (IDictionary)user.Value;
                   
                    playerIdList.Add(dictUser["userId"].ToString());
                    print(playerIdList.Count);
                }
            }
        });

    }

    public void PrintPlayerIdList()
    {
        
        print (playerIdList.Count);
        foreach (string list in playerIdList) {
            print(list);
        }
        
    }



}
