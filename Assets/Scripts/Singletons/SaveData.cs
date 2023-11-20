using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;

    //bench stuff
    public string benchSceneName;
    public Vector2 benchPos;

    //player stuff
    public int playerHealth;
    public int playerHeartShards;
    public float playerMana;
    public int playerManaOrbs;
    public int playerOrbShard;
    public float playerOrb0fill, playerOrb1fill, playerOrb2fill;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;

    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump;
    public bool playerUnlockedSideCast, playerUnlockedUpCast, playerUnlockedDownCast;
    
    //serialized heart shards
    public bool playerUnlockedHeartShard1, playerUnlockedHeartShard2, playerUnlockedHeartShard3, playerUnlockedHeartShard4, playerUnlockedHeartShard5, 
    playerUnlockedHeartShard6, playerUnlockedHeartShard7, playerUnlockedHeartShard8, playerUnlockedHeartShard9, playerUnlockedHeartShard10, 
    playerUnlockedHeartShard11, playerUnlockedHeartShard12, playerUnlockedHeartShard13, playerUnlockedHeartShard14, playerUnlockedHeartShard15, 
    playerUnlockedHeartShard16, playerUnlockedHeartShard17, playerUnlockedHeartShard18, playerUnlockedHeartShard19, playerUnlockedHeartShard20;

    //serialized orb shards
    public bool playerUnlockedOrbShard1, playerUnlockedOrbShard2, playerUnlockedOrbShard3, playerUnlockedOrbShard4, playerUnlockedOrbShard5,
    playerUnlockedOrbShard6, playerUnlockedOrbShard7, playerUnlockedOrbShard8, playerUnlockedOrbShard9;



    //enemies stuff
    //shade
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;



    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.bench.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.shade.data"));
        }

        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
    #region Bench Stuff
    public void SaveBench()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }
    public void LoadBench()
    {
        if(File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
        else
        {
            Debug.Log("Bench doesnt exist");
        }
    }
    #endregion

    #region Player stuff
    public void SavePlayerData()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);

            playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);

            int maxHealth = PlayerController.Instance.maxHealth;
            writer.Write(maxHealth);

            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerController.Instance.halfMana;
            writer.Write(playerHalfMana);
            playerManaOrbs = PlayerController.Instance.manaOrbs;
            writer.Write(playerManaOrbs);
            playerOrbShard = PlayerController.Instance.orbShard;
            writer.Write(playerOrbShard);
            playerOrb0fill = PlayerController.Instance.manaOrbsHandler.orbFills[0].fillAmount;
            writer.Write(playerOrb0fill);
            playerOrb1fill = PlayerController.Instance.manaOrbsHandler.orbFills[1].fillAmount;
            writer.Write(playerOrb1fill);
            playerOrb2fill = PlayerController.Instance.manaOrbsHandler.orbFills[2].fillAmount;
            writer.Write(playerOrb2fill);

            playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);
            playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);
            playerUnlockedVarJump = PlayerController.Instance.unlockedVarJump;
            writer.Write(playerUnlockedVarJump);

            playerUnlockedSideCast = PlayerController.Instance.unlockedSideCast;
            writer.Write(playerUnlockedSideCast);
            playerUnlockedUpCast = PlayerController.Instance.unlockedUpCast;
            writer.Write(playerUnlockedUpCast);
            playerUnlockedDownCast = PlayerController.Instance.unlockedDownCast;
            writer.Write(playerUnlockedDownCast);
            //each heart shard
            playerUnlockedHeartShard1 = PlayerController.Instance.unlockedHeartShard1;
            writer.Write(playerUnlockedHeartShard1);
            playerUnlockedHeartShard2 = PlayerController.Instance.unlockedHeartShard2;
            writer.Write(playerUnlockedHeartShard2);
            playerUnlockedHeartShard3 = PlayerController.Instance.unlockedHeartShard3;
            writer.Write(playerUnlockedHeartShard3);
            playerUnlockedHeartShard4 = PlayerController.Instance.unlockedHeartShard4;
            writer.Write(playerUnlockedHeartShard4);
            playerUnlockedHeartShard5 = PlayerController.Instance.unlockedHeartShard5;
            writer.Write(playerUnlockedHeartShard5);
            playerUnlockedHeartShard6 = PlayerController.Instance.unlockedHeartShard6;
            writer.Write(playerUnlockedHeartShard6);
            playerUnlockedHeartShard7 = PlayerController.Instance.unlockedHeartShard7;
            writer.Write(playerUnlockedHeartShard7);
            playerUnlockedHeartShard8 = PlayerController.Instance.unlockedHeartShard8;
            writer.Write(playerUnlockedHeartShard8);
            playerUnlockedHeartShard9 = PlayerController.Instance.unlockedHeartShard9;
            writer.Write(playerUnlockedHeartShard9);
            playerUnlockedHeartShard10 = PlayerController.Instance.unlockedHeartShard10;
            writer.Write(playerUnlockedHeartShard10);
            playerUnlockedHeartShard11 = PlayerController.Instance.unlockedHeartShard11;
            writer.Write(playerUnlockedHeartShard11);
            playerUnlockedHeartShard12 = PlayerController.Instance.unlockedHeartShard12;
            writer.Write(playerUnlockedHeartShard12);
            playerUnlockedHeartShard13 = PlayerController.Instance.unlockedHeartShard13;
            writer.Write(playerUnlockedHeartShard13);
            playerUnlockedHeartShard14 = PlayerController.Instance.unlockedHeartShard14;
            writer.Write(playerUnlockedHeartShard14);
            playerUnlockedHeartShard15 = PlayerController.Instance.unlockedHeartShard15;
            writer.Write(playerUnlockedHeartShard15);
            playerUnlockedHeartShard16 = PlayerController.Instance.unlockedHeartShard16;
            writer.Write(playerUnlockedHeartShard16);
            playerUnlockedHeartShard17 = PlayerController.Instance.unlockedHeartShard17;
            writer.Write(playerUnlockedHeartShard17);
            playerUnlockedHeartShard18 = PlayerController.Instance.unlockedHeartShard18;
            writer.Write(playerUnlockedHeartShard18);
            playerUnlockedHeartShard19 = PlayerController.Instance.unlockedHeartShard19;
            writer.Write(playerUnlockedHeartShard19);
            playerUnlockedHeartShard20 = PlayerController.Instance.unlockedHeartShard20;
            writer.Write(playerUnlockedHeartShard20);
            //each orb shard
            playerUnlockedOrbShard1 = PlayerController.Instance.unlockedOrbShard1;
            writer.Write(playerUnlockedOrbShard1);
            playerUnlockedOrbShard2 = PlayerController.Instance.unlockedOrbShard2;
            writer.Write(playerUnlockedOrbShard2);
            playerUnlockedOrbShard3 = PlayerController.Instance.unlockedOrbShard3;
            writer.Write(playerUnlockedOrbShard3);
            playerUnlockedOrbShard4 = PlayerController.Instance.unlockedOrbShard4;
            writer.Write(playerUnlockedOrbShard4);
            playerUnlockedOrbShard5 = PlayerController.Instance.unlockedOrbShard5;
            writer.Write(playerUnlockedOrbShard5);
            playerUnlockedOrbShard6 = PlayerController.Instance.unlockedOrbShard6;
            writer.Write(playerUnlockedOrbShard6);
            playerUnlockedOrbShard7 = PlayerController.Instance.unlockedOrbShard7;
            writer.Write(playerUnlockedOrbShard7);
            playerUnlockedOrbShard8 = PlayerController.Instance.unlockedOrbShard8;
            writer.Write(playerUnlockedOrbShard8);
            playerUnlockedOrbShard9 = PlayerController.Instance.unlockedOrbShard9;
            writer.Write(playerUnlockedOrbShard9);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
        Debug.Log("saved player data");
        

    }
    public void LoadPlayerData()
    {
        if(File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerHeartShards = reader.ReadInt32();

                int maxHealth = reader.ReadInt32();
                PlayerController.Instance.maxHealth = maxHealth;

                int additionalHealth = playerHeartShards / 4;  // Assuming 4 heart shards add 1 health
                PlayerController.Instance.maxHealth += additionalHealth;
                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();
                playerManaOrbs = reader.ReadInt32();
                playerOrbShard = reader.ReadInt32();
                playerOrb0fill = reader.ReadSingle();
                playerOrb1fill = reader.ReadSingle();
                playerOrb2fill = reader.ReadSingle();

                playerUnlockedWallJump = reader.ReadBoolean();
                playerUnlockedDash = reader.ReadBoolean();
                playerUnlockedVarJump = reader.ReadBoolean();

                playerUnlockedSideCast = reader.ReadBoolean();
                playerUnlockedUpCast = reader.ReadBoolean();
                playerUnlockedDownCast = reader.ReadBoolean();
                
                //each heart shard
                playerUnlockedHeartShard1 = reader.ReadBoolean();
                playerUnlockedHeartShard2 = reader.ReadBoolean();
                playerUnlockedHeartShard3 = reader.ReadBoolean();
                playerUnlockedHeartShard4 = reader.ReadBoolean();
                playerUnlockedHeartShard5 = reader.ReadBoolean();
                playerUnlockedHeartShard6 = reader.ReadBoolean();
                playerUnlockedHeartShard7 = reader.ReadBoolean();
                playerUnlockedHeartShard8 = reader.ReadBoolean();
                playerUnlockedHeartShard9 = reader.ReadBoolean();
                playerUnlockedHeartShard10 = reader.ReadBoolean();
                playerUnlockedHeartShard11 = reader.ReadBoolean();
                playerUnlockedHeartShard12 = reader.ReadBoolean();
                playerUnlockedHeartShard13 = reader.ReadBoolean();
                playerUnlockedHeartShard14 = reader.ReadBoolean();
                playerUnlockedHeartShard15 = reader.ReadBoolean();
                playerUnlockedHeartShard16 = reader.ReadBoolean();
                playerUnlockedHeartShard17 = reader.ReadBoolean();
                playerUnlockedHeartShard18 = reader.ReadBoolean();
                playerUnlockedHeartShard19 = reader.ReadBoolean();
                playerUnlockedHeartShard20 = reader.ReadBoolean();

                //each mana orb
                playerUnlockedOrbShard1 = reader.ReadBoolean();
                playerUnlockedOrbShard2 = reader.ReadBoolean();
                playerUnlockedOrbShard3 = reader.ReadBoolean();
                playerUnlockedOrbShard4 = reader.ReadBoolean();
                playerUnlockedOrbShard5 = reader.ReadBoolean();
                playerUnlockedOrbShard6 = reader.ReadBoolean();
                playerUnlockedOrbShard7 = reader.ReadBoolean();
                playerUnlockedOrbShard8 = reader.ReadBoolean();
                playerUnlockedOrbShard9 = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();

                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;
                PlayerController.Instance.halfMana = playerHalfMana;
                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.heartShards = playerHeartShards;
                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.manaOrbs = playerManaOrbs;
                PlayerController.Instance.orbShard = playerOrbShard;
                PlayerController.Instance.manaOrbsHandler.orbFills[0].fillAmount = playerOrb0fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[1].fillAmount = playerOrb1fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[2].fillAmount = playerOrb2fill;

                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedVarJump = playerUnlockedVarJump;

                PlayerController.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerController.Instance.unlockedUpCast = playerUnlockedUpCast;
                PlayerController.Instance.unlockedDownCast = playerUnlockedDownCast;
                
                //each heart shard
                PlayerController.Instance.unlockedHeartShard1 = playerUnlockedHeartShard1;
                PlayerController.Instance.unlockedHeartShard2 = playerUnlockedHeartShard2;
                PlayerController.Instance.unlockedHeartShard3 = playerUnlockedHeartShard3;
                PlayerController.Instance.unlockedHeartShard4 = playerUnlockedHeartShard4;
                PlayerController.Instance.unlockedHeartShard5 = playerUnlockedHeartShard5;
                PlayerController.Instance.unlockedHeartShard6 = playerUnlockedHeartShard6;
                PlayerController.Instance.unlockedHeartShard7 = playerUnlockedHeartShard7;
                PlayerController.Instance.unlockedHeartShard8 = playerUnlockedHeartShard8;
                PlayerController.Instance.unlockedHeartShard9 = playerUnlockedHeartShard9;
                PlayerController.Instance.unlockedHeartShard10 = playerUnlockedHeartShard10;
                PlayerController.Instance.unlockedHeartShard11 = playerUnlockedHeartShard11;
                PlayerController.Instance.unlockedHeartShard12 = playerUnlockedHeartShard12;
                PlayerController.Instance.unlockedHeartShard13 = playerUnlockedHeartShard13;
                PlayerController.Instance.unlockedHeartShard14 = playerUnlockedHeartShard14;
                PlayerController.Instance.unlockedHeartShard15 = playerUnlockedHeartShard15;
                PlayerController.Instance.unlockedHeartShard16 = playerUnlockedHeartShard16;
                PlayerController.Instance.unlockedHeartShard17 = playerUnlockedHeartShard17;
                PlayerController.Instance.unlockedHeartShard18 = playerUnlockedHeartShard18;
                PlayerController.Instance.unlockedHeartShard19 = playerUnlockedHeartShard19;
                PlayerController.Instance.unlockedHeartShard20 = playerUnlockedHeartShard20;

                //each mana orb
                PlayerController.Instance.unlockedOrbShard1 = playerUnlockedOrbShard1;
                PlayerController.Instance.unlockedOrbShard2 = playerUnlockedOrbShard2;
                PlayerController.Instance.unlockedOrbShard3 = playerUnlockedOrbShard3;
                PlayerController.Instance.unlockedOrbShard4 = playerUnlockedOrbShard4;
                PlayerController.Instance.unlockedOrbShard5 = playerUnlockedOrbShard5;
                PlayerController.Instance.unlockedOrbShard6 = playerUnlockedOrbShard6;
                PlayerController.Instance.unlockedOrbShard7 = playerUnlockedOrbShard7;
                PlayerController.Instance.unlockedOrbShard8 = playerUnlockedOrbShard8;
                PlayerController.Instance.unlockedOrbShard9 = playerUnlockedOrbShard9;
            }
            Debug.Log("load player data");
            Debug.Log(playerHalfMana);
        }
        else
        {
            Debug.Log("File doesnt exist");
            PlayerController.Instance.halfMana = false;
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.Mana = 0.5f;
            PlayerController.Instance.heartShards = 0;

            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
            PlayerController.Instance.unlockedVarJump = false;

            PlayerController.Instance.unlockedSideCast = false;
            PlayerController.Instance.unlockedUpCast = false;
            PlayerController.Instance.unlockedDownCast = false;

            //each heart shard
            PlayerController.Instance.unlockedHeartShard1 = false;
            PlayerController.Instance.unlockedHeartShard2 = false;
            PlayerController.Instance.unlockedHeartShard3 = false;
            PlayerController.Instance.unlockedHeartShard4 = false;
            PlayerController.Instance.unlockedHeartShard5 = false;
            PlayerController.Instance.unlockedHeartShard6 = false;
            PlayerController.Instance.unlockedHeartShard7 = false;
            PlayerController.Instance.unlockedHeartShard8 = false;
            PlayerController.Instance.unlockedHeartShard9 = false;
            PlayerController.Instance.unlockedHeartShard10 = false;
            PlayerController.Instance.unlockedHeartShard11 = false;
            PlayerController.Instance.unlockedHeartShard12 = false;
            PlayerController.Instance.unlockedHeartShard13 = false;
            PlayerController.Instance.unlockedHeartShard14 = false;
            PlayerController.Instance.unlockedHeartShard15 = false;
            PlayerController.Instance.unlockedHeartShard16 = false;
            PlayerController.Instance.unlockedHeartShard17 = false;
            PlayerController.Instance.unlockedHeartShard18 = false;
            PlayerController.Instance.unlockedHeartShard19 = false;
            PlayerController.Instance.unlockedHeartShard20 = false;

            //each orb shard
            PlayerController.Instance.unlockedOrbShard1 = false;
            PlayerController.Instance.unlockedOrbShard2 = false;
            PlayerController.Instance.unlockedOrbShard3 = false;
            PlayerController.Instance.unlockedOrbShard4 = false;
            PlayerController.Instance.unlockedOrbShard5 = false;
            PlayerController.Instance.unlockedOrbShard6 = false;
            PlayerController.Instance.unlockedOrbShard7 = false;
            PlayerController.Instance.unlockedOrbShard8 = false;
            PlayerController.Instance.unlockedOrbShard9 = false;
        }
    }

    #endregion

    #region enemy stuff
    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);

            writer.Write(shadePos.x);
            writer.Write(shadePos.y);

            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }
    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.data")))
            {
                sceneWithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                shadeRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            }
            Debug.Log("Load shade data");
        }
        else
        {
            Debug.Log("Shade doesnt exist");
        }
    }
    #endregion
    public void ClearSavedData()
{
    // Delete the save files if they exist
    if (File.Exists(Application.persistentDataPath + "/save.bench.data"))
    {
        File.Delete(Application.persistentDataPath + "/save.bench.data");
    }

    if (File.Exists(Application.persistentDataPath + "/save.player.data"))
    {
        File.Delete(Application.persistentDataPath + "/save.player.data");
    }

    if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
    {
        File.Delete(Application.persistentDataPath + "/save.shade.data");
    }

 
}

// ...

// Call this method when you want to clear the saved data
// For example, when the player selects a "New Game" option
public void NewGame()
{
    ClearSavedData();
    // Additional initialization for a new game, if needed
}

}
