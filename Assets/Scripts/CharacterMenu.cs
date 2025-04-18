using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour {
    // text fields
    public Text levelText, hitpointText, pesosText, upgradeCostText, xpText;

    // logic
    private int currentCharacterSelection = 0;
    public Image characterSelectionSprite;
    public Image weaponSprite;
    public RectTransform xpBar;

    // character selection
    public void OnArrowClick(bool right) {
        if (right) {
            currentCharacterSelection++;

            // if we went too far away
            if (currentCharacterSelection == GameManager.instance.playerSprites.Count) {
                currentCharacterSelection = 0;
            }

            OnSelectionChanged();
        } else {
            currentCharacterSelection--;

            // if we went too far away
            if (currentCharacterSelection < 0) {
                currentCharacterSelection = GameManager.instance.playerSprites.Count - 1;
            }

            OnSelectionChanged();
        }
    }

    public void OnSelectionChanged() {
        characterSelectionSprite.sprite = GameManager.instance.playerSprites[currentCharacterSelection];
        GameManager.instance.player.SwapSprite(currentCharacterSelection);
    }

    // weapon upgrade
    public void OnUpgradeClick() {
        if (GameManager.instance.TryUpgradeWeapon()) {
            UpdateMenu();
        }
    }
    
    // update character info
    public void UpdateMenu() {
        // weapon
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLevel];
        if (GameManager.instance.weapon.weaponLevel == GameManager.instance.weaponPrices.Count) {
            upgradeCostText.text = "MAX";
        }  else {
            upgradeCostText.text = GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLevel].ToString();
        }

        // meta
        levelText.text = GameManager.instance.GetCurrentLevel().ToString();
        hitpointText.text = GameManager.instance.player.hitpoint.ToString();
        pesosText.text = GameManager.instance.pesos.ToString();

        // xp bar
        int currLevel = GameManager.instance.GetCurrentLevel();
        if (currLevel == GameManager.instance.xpTable.Count) {
            xpText.text = GameManager.instance.experience.ToString() + " total experience points"; // display total xp
            xpBar.localScale = Vector3.one;
        } else {
            int prevLevelXp = GameManager.instance.GetXpToLevel(currLevel - 1);
            int currLevelXp = GameManager.instance.GetXpToLevel(currLevel);

            int diff = currLevelXp - prevLevelXp;
            int currXpIntoLevel = GameManager.instance.experience - prevLevelXp;

            float completionRatio = (float)currXpIntoLevel / (float)diff;
            xpBar.localScale = new Vector3(completionRatio, 1, 1);
            xpText.text = currXpIntoLevel.ToString() + " / " + diff;
        }
    }
}
