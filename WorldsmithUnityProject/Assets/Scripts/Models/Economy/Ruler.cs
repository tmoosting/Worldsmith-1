﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ruler : EcoBlock
{
    
    public enum Hierarchy {  Unassigned, Dominating, Independent, Dominated, Secondary   }


    public Hierarchy rulerHierarchy = Hierarchy.Unassigned;

    public bool isLocalRuler;

    public Ruler(Location loc, bool localRuler)
    {
        isLocalRuler = localRuler;
        if (isLocalRuler)
            loc.localRuler = this;
        else
            loc.secondaryRulers.Add(this);
        xLocation = loc.GetPositionVector().x;
        yLocation = loc.GetPositionVector().y;
        blockType = BlockType.Ruler;

        blockID = "Ruler";
        int nr = 1;
        if (isLocalRuler)
            blockID = "Primary";
        else
            blockID = "Secondary";
        blockID += "RulerOf" + loc.elementID;
        blockID += nr;
         
        while (EconomyController.Instance.BlockIDExists(this.blockID, this.blockType) == true)
        {
            nr++;
            if (isLocalRuler)
                blockID = "Primary";
            else
                blockID = "Secondary";
            blockID += "RulerOf" + loc.elementID;
            blockID += nr;
        }

    }

    public Location GetHomeLocation()
    {      
       foreach (Location loc in WorldController.Instance.GetWorld().locationList)
        {
            if (loc.localRuler == this)
                return loc;
            if (loc.secondaryRulers.Contains(this))
                return loc;
        }    
        Debug.Log("Failed to get Location for Ruler " + blockID);
        return null;
    }
     
     

    public List<Ruler> GetControlledRulers()
    {
        List<Ruler> returnList = new List<Ruler>();
        foreach (Ruler ruler in EconomyController.Instance.rulerDictionary.Keys)        
            if (EconomyController.Instance.rulerDictionary[ruler] == this)
                returnList.Add(ruler);
        return returnList;     
    }
    public List<Population> GetControlledPopulations()
    {
        List<Population> returnList = new List<Population>();
        foreach (Population civasset in EconomyController.Instance.populationDictionary.Keys)        
            if (EconomyController.Instance.populationDictionary[civasset].blockID == blockID)            
                returnList.Add(civasset);
        return returnList;
    }
    public List<Warband> GetControlledWarbands()
    {
        List<Warband> returnList = new List<Warband>();
        foreach (Warband warband in EconomyController.Instance.warbandDictionary.Keys) 
            if (EconomyController.Instance.warbandDictionary[warband] != null)
               if (EconomyController.Instance.warbandDictionary[warband].blockID == blockID)
                  returnList.Add(warband);
        return returnList;
    }
    public List<Location> GetControlledLocations()
    {
        List<Location> returnList = new List<Location>();
        foreach (Ruler contruler in GetControlledRulers() )
            if (contruler.isLocalRuler == true)
                returnList.Add(contruler.GetHomeLocation());

        foreach (Location loc in WorldController.Instance.GetWorld().locationList)        
            if (loc.dominatingRuler == this)
                if (returnList.Contains(loc) == false) 
                    returnList.Add(loc); 

        return returnList;
    }

    public Ruler GetController()
    { 

        if (EconomyController.Instance.rulerDictionary[this] == null)
            return this;
        else
            return EconomyController.Instance.rulerDictionary[this];
    }

}
