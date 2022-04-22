using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode; //This is the item code for the corresponding seed
    public int[] growthDays; //Days growth for each stage
    public int totalGrowthDays; //Total growth days
    public GameObject[] growthPrefab; //Prefab to use when instantiating growth stages
    public Sprite[] growthSprite; //Growth sprite
    public Season[] seasons; //Growth seasons
    public Sprite harvestedSprite; //Sprite used once harvested
    [ItemCodeDescription]
    public int harvestedTransformItemCode; //If the item transforms into another item when harvested this item code will be populated
    public bool hideCropBeforeHarvestedAnimation; //If the crop should be disabled before the harvested animation
    public bool disableCropCollidersBeforeHarvestedAnimaiton; //If colliders on crop should be disabled to avoid the harvested animation effecting any other game objects
    public bool isHarvestedAnimation; //True if harvested animation to be played on final growth stage prefab
    public bool isHarvestActionEffect = false; //Flag to determine whether there is a harvest action effect
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect; //The harvest action effect for the crop

    [ItemCodeDescription]
    public int[] harvestToolItemCode; //Array of item codes for the tools that can harvest or 0 array elements if no tool required
    public int[] requiredHarvestActions; //Numbers of harvest actions required for corresponding tool in harvest tool item code array
    [ItemCodeDescription]
    public int[] cropProducedItemCode; //Array of item codes produced for the harvested crop
    public int[] cropProducedMinQuantity; //Array of minumum quantities produced for the harvested crop
    public int[] cropProducedMaxQuantity; //If max quantity is > min quantity then a random number number of crops between min and max are produced
    public int daysToRegrow; //Days to regrow next crop or -1 if a single crop

    /// <summary>
    /// Returns true if the tool item code can be used to harvest this crop, else returns false
    /// </summary>
    /// <param name="toolItemCode"></param>
    /// <returns></returns>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Returns -1 if the tool can`t be used to harvest this crop, else returns the number of harvest actions required by this tool
    /// </summary>
    /// <param name="toolItemcode"></param>
    /// <returns></returns>
    public int RequiredHarvestActionsForTool(int toolItemcode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemcode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
