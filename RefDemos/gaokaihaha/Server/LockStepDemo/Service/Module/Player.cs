﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player
{
    public SyncSession session;
    public string playerID;
    public string nickName = "";

    public string characterID = "1";
    public List<string> OwnCharacter = new List<string>();

    public int Coin = 0;
    public int Diamond = 0;
}
