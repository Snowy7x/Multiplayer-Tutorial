using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class Menu
    {
        public string name;
        public GameObject gameObject;
    }

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public List<Menu> menus;
    public string initialMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        OpenMenu(initialMenu);
    }


    public void OpenMenu(string name)
    {
        foreach (Menu menu in menus)
        {
            if (menu.name == name)
            {
                menu.gameObject.SetActive(true);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }
        }
    }

    public void CloseMenu(string name)
    {
        foreach (Menu menu in menus)
        {
            if (menu.name == name)
            {
                menu.gameObject.SetActive(false);
            }
        }
    }

    public void CloseAllMenus()
    {
        foreach (Menu menu in menus)
        {
            menu.gameObject.SetActive(false);
        }
    }

    public void ToggleMenu(string name)
    {
        foreach (Menu menu in menus)
        {
            if (menu.name == name)
            {
                if (menu.gameObject.activeSelf)
                {
                    menu.gameObject.SetActive(false);
                }
                else
                {
                    menu.gameObject.SetActive(true);
                }
            }
        }
    }

    public string GetActiveMenu()
    {
        foreach (Menu menu in menus)
        {
            if (menu.gameObject.activeSelf)
            {
                return menu.name;
            }
        }

        return null;
    }
}



