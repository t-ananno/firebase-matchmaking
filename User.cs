using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class User  {

   public   string userName;
   public  string userId;
   public  string userSearching;
   public  string userInGame;

   


    public User()   //Write anything , pushed to the Firebase server
    {
        
        userName = "Player "+MatchMaking.num;
        userId = MatchMaking.num.ToString();
        userSearching = JoinGame.searchingChecker;
        userInGame = JoinGame.InGameChecker;
    }
}
