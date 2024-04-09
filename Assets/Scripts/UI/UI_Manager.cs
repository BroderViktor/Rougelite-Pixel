using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public GameObject SpellMenu, Menu;
    
    public GameObject[] spellMenus;
    int currentMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !Menu.activeSelf)
        {
            SpellMenuEnterExit();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);
            Time.timeScale = Menu.activeSelf ? 0 : 1;
            global.userInput = !Menu.activeSelf;
        }
    }
    public void SpellMenuEnterExit()
    {
        SpellMenu.SetActive(!SpellMenu.activeSelf);
        if (SpellMenu.activeSelf) SpellMenu.GetComponent<CreateSpell>().CreateNewSpell();
        global.userInput = !SpellMenu.activeSelf;
        Time.timeScale = SpellMenu.activeSelf ? 0 : 1;
        spellMenus[currentMenu].SetActive(false);
        currentMenu = 0;
        spellMenus[currentMenu].SetActive(true);
    }
    public void LoadNextMenu()
    {
        spellMenus[currentMenu].SetActive(false);
        currentMenu++;
        spellMenus[currentMenu].SetActive(true);
    }
    public void LoadNextMenuWithInt(int newMenuNum)
    {
        spellMenus[currentMenu].SetActive(false);
        currentMenu = newMenuNum;
        spellMenus[currentMenu].SetActive(true);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
