using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }
        public Location CurrentLocation { get; set; }

        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;
            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if(location.ItemRequiredToEnter == null)
            {
                return true;
            }
            foreach(InventoryItem inventoryItem in Inventory)
            {
                if(inventoryItem.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasThisQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(playerQuest.Details.ID == quest.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsCompleted;
                }
            }
            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach(QuestCompletionItem questCompletionItem in quest.QuestCompletionItems)
            {
                bool found = false;

                foreach(InventoryItem inventoryItem in Inventory)
                {
                    if(inventoryItem.Details.ID == questCompletionItem.Details.ID)
                    {
                        found = true;
                        if(inventoryItem.Quantity < questCompletionItem.Quantity)
                        {
                            return false;
                        }
                    }
                }

                if (!found)
                {
                    return false;
                }

            }
            return true;
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            foreach (InventoryItem item in Inventory)
            {
                if(item.Details.ID == itemToAdd.ID)
                {
                    item.Quantity++;
                    return;
                }
            }
            Inventory.Add(new InventoryItem(itemToAdd,1));
        }

        public void MarkQuestAsCompleted(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(playerQuest.Details.ID == quest.ID)
                {
                    playerQuest.IsCompleted = true;
                    return;
                }
            }
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem questCompletionItem in quest.QuestCompletionItems)
            {
                foreach (InventoryItem item in Inventory)
                {
                    if (item.Details.ID == questCompletionItem.Details.ID)
                    {
                        item.Quantity -= questCompletionItem.Quantity;
                        break;
                    }
                }
            }
        }

        public void RemoveItemFromInventory(Item itemToRemove,int quantity = 1)
        {
            foreach (InventoryItem item in Inventory)
            {
                if (item.Details.ID == itemToRemove.ID)
                {
                    item.Quantity -= quantity;
                    break;
                }
            }
        }
    }
}
