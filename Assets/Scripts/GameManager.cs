using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            Destroy(player);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    // resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;
    public RectTransform hitpointBar;
    // references
    public Player player;
    // public weapon
    public Weapon weapon;
    public Animator deathMenuAnim;
    public FloatingTextManager floatingTextManager;
    public GameObject hud;
    public GameObject menu;
    // logic
    public int pesos;
    public int experience;

    // floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration) {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    // upgrade weapon
    public bool TryUpgradeWeapon() {
        // is weapon max level?
        if (weaponPrices.Count <= weapon.weaponLevel) {
            return false;
        } 

        if (pesos >= weaponPrices[weapon.weaponLevel]) {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }

    // hitpoint bar
    public void OnHitpointChange() {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // experience state
    public int GetCurrentLevel() {
        int r = 0;
        int add = 0;

        while (experience >= add) {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) { // max lvl
                return r;
            }
        }

        return r;
    }


    public int GetXpToLevel(int level) {
        int r = 0;
        int xp = 0;

        while (r < level) {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }

    public void GrantXp(int xp) {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel()) {
            OnLevelUp();
        }
    }
    
    public void OnLevelUp() {
        Debug.Log("Level up!");
        player.OnLevelUp();
        OnHitpointChange();
    }

    // on scene loaded
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene s, LoadSceneMode mode) {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    // death menu & respawn
    public void Respawn() {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // save state
    /*
     * INT preferredSkin
     * INT pesos
     * INT experience
     * INT weaponLevel
     */
    public void SaveState() {
        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();
    }

    public void LoadState(UnityEngine.SceneManagement.Scene s, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState")) {
            return;
        }

        string[] data = PlayerPrefs.GetString("SaveState").Split("|");

        // change player skin
        pesos = int.Parse(data[1]);

        // experience
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1) {
            player.SetLevel(GetCurrentLevel());
        }

        // change weapon level
        weapon.SetWeaponLevel(int.Parse(data[3]));
    }
}
