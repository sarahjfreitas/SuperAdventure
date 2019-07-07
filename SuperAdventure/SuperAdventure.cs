using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player player;
        private Monster currentMonster;
        public SuperAdventure()
        {
            InitializeComponent();

            player = new Player(10,10,20,0,1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD),1));
            RefreshPlayerInventoryList();
        }
        

        private void BtnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToNorth);
        }

        private void BtnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToSouth);
        }

        private void BtnEast_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToEast);
        }

        private void BtnWest_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToWest);
        }

        private void MoveTo(Location newLocation)
        {
            // Does the location have any required items
            if (!player.HasRequiredItemToEnterThisLocation(newLocation))
            {                
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }

            // update player location
            player.CurrentLocation = newLocation;

            // show/hide buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            // display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // heal the player
            player.CurrentHitPoints = player.MaximumHitPoints;
            lblHitPoints.Text = player.CurrentHitPoints.ToString();


            // check if location has quest
            if(newLocation.QuestAvailableHere != null)
            {
                if (player.HasThisQuest(newLocation.QuestAvailableHere))
                {
                    if (!player.CompletedThisQuest(newLocation.QuestAvailableHere))
                    {
                        if (player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere))
                        {
                            rtbMessages.Text += Environment.NewLine + "You complete the '" + newLocation.QuestAvailableHere.Name + "' quest" + Environment.NewLine;
                            player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);
                            player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            player.Gold += newLocation.QuestAvailableHere.RewardGold;
                            player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);
                            player.MarkQuestAsCompleted(newLocation.QuestAvailableHere);

                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;                            
                        }
                    }
                }
                else //player dont have the quest
                {
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
                    foreach(QuestCompletionItem item in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if(item.Quantity == 1)
                        {
                            rtbMessages.Text += item.Quantity + " " + item.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += item.Quantity + " " + item.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;
                    player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
                
            }

            // check for monster
            if(newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;
                Monster modelMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);
                currentMonster = new Monster(modelMonster.ID, modelMonster.Name, modelMonster.MaximumDamage, modelMonster.RewardExperiencePoints,
                    modelMonster.RewardGold, modelMonster.MaximumHitPoints, modelMonster.MaximumHitPoints);
                foreach(LootItem lootItem in modelMonster.LootTable)
                {
                    currentMonster.LootTable.Add(lootItem);
                }
                ShowCombatControls();
            }
            else
            {
                currentMonster = null;
                HideCombatControls();
            }
            RefreshPlayerQuestsList();
            RefreshPlayerWeaponsList();
            RefreshPlayerInventoryList();
            RefreshPlayerPotionsList();
            refreshPlayerStatus();
        }

        private void ShowCombatControls()
        {
            cboWeapons.Visible = true;
            cboPotions.Visible = true;
            btnUseWeapon.Visible = true;
            btnUsePotion.Visible = true;
        }

        private void HideCombatControls()
        {
            cboWeapons.Visible = false;
            cboPotions.Visible = false;
            btnUseWeapon.Visible = false;
            btnUsePotion.Visible = false;
        }

        private void RefreshPlayerInventoryList()
        {
            // refresh player inventory list
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";
            dgvInventory.Rows.Clear();
            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
        }

        private void RefreshPlayerPotionsList()
        {
            // Refresh player's potions combobox
            List<HealingPotion> healingPotions = new List<HealingPotion>();
            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }
            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";
                cboPotions.SelectedIndex = 0;
            }
        }

        private void RefreshPlayerWeaponsList()
        {
            // Refresh player's weapons combobox
            List<Weapon> weapons = new List<Weapon>();
            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }
            if (weapons.Count == 0)
            {
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";
                cboWeapons.SelectedIndex = 0;
            }
        }

        private void RefreshPlayerQuestsList()
        {
            // refresh player quest list
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";
            dgvQuests.Rows.Clear();
            foreach (PlayerQuest playerQuest in player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }
        }

        private void BtnUseWeapon_Click(object sender, EventArgs e)
        {
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // hit monster
            int damageToMoster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);
            currentMonster.CurrentHitPoints -= damageToMoster;
            rtbMessages.Text += "You hit the " + currentMonster.Name + " for " + damageToMoster.ToString() + " points" + Environment.NewLine;

            // check if monster is dead
            if(currentMonster.CurrentHitPoints <= 0)
            {
                rtbMessages.Text += Environment.NewLine + "You defeated the " + currentMonster.Name + Environment.NewLine;
                player.ExperiencePoints += currentMonster.RewardExperiencePoints;
                rtbMessages.Text += "You receive " + currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                rtbMessages.Text += "You receive " + currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;

                // check and add droped items
                List<InventoryItem> lootedItems = new List<InventoryItem>();
                foreach(LootItem lootItem in currentMonster.LootTable)
                {
                    if (lootItem.IsDefaultItem)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                    else if (RandomNumberGenerator.NumberBetween(1,100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                foreach(InventoryItem inventoryItem in lootedItems)
                {
                    player.AddItemToInventory(inventoryItem.Details);
                    if(inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity + " " + inventoryItem.Details.Name + Environment.NewLine;
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                    }
                }

                RefreshPlayerInventoryList();
                RefreshPlayerPotionsList();
                RefreshPlayerWeaponsList();

                rtbMessages.Text += Environment.NewLine;

                MoveTo(player.CurrentLocation);
            }
            else // monster is alive
            {
                MonsterAtack();
            }

        }

        private void BtnUsePotion_Click(object sender, EventArgs e)
        {
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            player.CurrentHitPoints += potion.AmountToHeal;
            if(player.CurrentHitPoints > player.MaximumHitPoints)
            {
                player.CurrentHitPoints = player.MaximumHitPoints;
            }
            player.RemoveItemFromInventory(potion);

            rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
            RefreshPlayerInventoryList();
            RefreshPlayerPotionsList();

            MonsterAtack();

        }

        private void MonsterAtack()
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);
            rtbMessages.Text += "The " + currentMonster.Name + " did " + damageToPlayer.ToString() + " damage to you" + Environment.NewLine;
            player.CurrentHitPoints -= damageToPlayer;
            lblHitPoints.Text = player.CurrentHitPoints.ToString();
            if (player.CurrentHitPoints <= 0)
            {
                rtbMessages.Text += "The " + currentMonster.Name + " killed you." + Environment.NewLine;
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
            refreshPlayerStatus();
        }

        private void RtbMessages_TextChanged(object sender, EventArgs e)
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void refreshPlayerStatus()
        {
            lblExperience.Text = player.ExperiencePoints.ToString();
            lblGold.Text = player.Gold.ToString();
            lblLevel.Text = player.Level.ToString();
            lblHitPoints.Text = player.CurrentHitPoints.ToString();
        }
    }
}
