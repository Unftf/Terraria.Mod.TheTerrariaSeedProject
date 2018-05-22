﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.World.Generation;

using Terraria;
using Terraria.ModLoader;
using Terraria.Map;
using System.IO;
using System.Reflection;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheTerrariaSeedProject;
using Terraria.ModLoader.UI;
using System.Threading;



namespace TheTerrariaSeedProject.UI
{
    internal class UISearchSettings : UIState
    {
        private UIGenProgressBar progressBar = new UIGenProgressBar();
        private UIHeader progressMessage = new UIHeader();
        private GenerationProgress progress;

        public UIPanel buttonPanel;

        public UIPanel optionPanel;
        public UIList optionList;

        public UIPanel detailsPanel;
        public UIListDescription detailsList;

        public UIImageButton searchButton;
        public UIImageButton optionsButton;
        public UIImageButton stopButton;
        public UIImageButton positiveButton;
        public UIImageButton configLoadButton;
        public UIImageButton configSaveButton;
        public UIImageButton clearButton;

        public UIScrollbar optionListScrollbar;
        public UIScrollbar detailsListScrollbar;
        public float currentSize = 0;

        WorldGenSeedSearch wgss;
        public OptionsDict opdict;
        public InfoPanel infopanel;


        

        const float panelWidth = 230;

        public Configuration currentConfig = null;

        public static class IconNames
        {
            public const string search = "@iconSearch";
            public const string options = "@iconOption";
            public const string configLoad = "@iconConfigLoad";
            public const string configSave = "@iconConfigSave";
            public const string positive = "@iconPositive";
            public const string reset = "@iconReset";
            public const string stop = "@iconStop";
        }


        public bool writeText = false;
        public bool writeTextUpdating = false;
        public string writtenText = "";
            /*
            "..# 00.01.02.03.04.05.06.07.08.09.10.11.12 |++ \n" +
            "-----------------------------\n" +
            ".0# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".1# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".2# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".3# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".4# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".5# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".6# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".7# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".8# 00.00.00.00.00.00.00.00.00.00.00.00.00 #00 \n" +
            ".9# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "10# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "11# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "12# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "-----------------------------\n" +
            "++: 14.23.39.48.56.65.73.80.91.12.11.12.13 :88 \n";*/

        public string countText = "";


        public bool writeStats = false;
        public string writtenStats = "";

        public Dictionary<string, Texture2D> iconDict;

        public UISearchSettings(GenerationProgress progress, Mod mod, WorldGenSeedSearch wgss)
        {
            this.wgss = wgss;

            iconDict = new Dictionary<string, Texture2D>();
            iconDict.Add(IconNames.search, mod.GetTexture("search"));
            iconDict.Add(IconNames.options, mod.GetTexture("options"));
            iconDict.Add(IconNames.configLoad, mod.GetTexture("configLoad"));
            iconDict.Add(IconNames.configSave, mod.GetTexture("configSave"));
            iconDict.Add(IconNames.positive, mod.GetTexture("positive"));
            iconDict.Add(IconNames.reset, Main.trashTexture);
            iconDict.Add(IconNames.stop, mod.GetTexture("stop"));

            
            searchButton = new UIImageButton(mod.GetTexture("search"));
            optionsButton = new UIImageButton(mod.GetTexture("options"));


            configLoadButton = new UIImageButton(mod.GetTexture("configLoad"));
            configSaveButton = new UIImageButton(mod.GetTexture("configSave"));
            //clearButton = new UIImageButton(mod.GetTexture("clear"));

            clearButton = new UIImageButton(Main.trashTexture);
            positiveButton = new UIImageButton(mod.GetTexture("positive"));

            stopButton = new UIImageButton(mod.GetTexture("stop"));

            


            detailsPanel = new UIPanel();
            detailsPanel.SetPadding(5);
            detailsPanel.HAlign = 1f;
            detailsPanel.VAlign = 0.5f;
            detailsPanel.MarginRight = 19;
            detailsPanel.Width.Set(0, 0.45f);
            detailsPanel.Height.Set(0f, 0.5f);
            detailsPanel.BackgroundColor = new Color(73, 94, 171);
            Append(detailsPanel);



            detailsListScrollbar = new UIScrollbar();
            detailsListScrollbar.SetView(100f, 1000f);
            detailsListScrollbar.Height.Set(-12f, 1f);
            detailsListScrollbar.HAlign = 1f;
            detailsListScrollbar.Top.Pixels = 6;
            detailsListScrollbar.MarginRight = 2;

            detailsList = new UIListDescription(this);
            detailsList.Width.Set(0, 1f);
            detailsList.Height.Set(0f, 1f);
            detailsList.ListPadding = 12f;
            detailsList.MarginRight = 10;
            detailsPanel.Append(detailsList);

            detailsPanel.Append(detailsListScrollbar);
            detailsList.SetScrollbar(detailsListScrollbar);

            detailsList.Width.Set(-detailsListScrollbar.MarginRight - detailsListScrollbar.Width.Pixels, 1f);
            detailsList.SetAlignWidth(getDescListWith());

            optionPanel = new UIPanel();
            optionPanel.SetPadding(5);
            optionPanel.HAlign = 0f;
            optionPanel.VAlign = 0.5f;
            optionPanel.Left.Pixels = 20;
            optionPanel.Width.Set(0, 0.45f);
            optionPanel.Height.Set(0f, 0.5f);
            optionPanel.BackgroundColor = new Color(73, 94, 171);


            
            optionListScrollbar = new UIScrollbar();
            optionListScrollbar.SetView(100f, 1000f);
            optionListScrollbar.Height.Set(-12f, 1f);
            optionListScrollbar.HAlign = 1.0f;
            optionListScrollbar.Top.Pixels = 6;
            optionListScrollbar.MarginRight = 2;

            optionList = new UIList();
            optionList.Width.Set(-optionListScrollbar.MarginRight - optionListScrollbar.Width.Pixels, 1f);
            optionList.Height.Set(0f, 1f);
            optionList.ListPadding = 12f;
            optionPanel.Append(optionList);

            optionPanel.Append(optionListScrollbar);
            optionList.SetScrollbar(optionListScrollbar);

            opdict = new OptionsDict();
            infopanel = new InfoPanel(optionList, detailsList, opdict);



            Append(optionPanel);

            float spacing = 3f;
            int spacingVFac = 4;
            buttonPanel = new UIPanel();
            buttonPanel.SetPadding(0);
            buttonPanel.HAlign = 0.5f;
            buttonPanel.VAlign = 0.5f;
            //buttonPanel.Top.Set(360f, 0f);			
            buttonPanel.Width.Set(searchButton.Width.Pixels + 2 * spacing, 0f);
            buttonPanel.BackgroundColor = new Color(73, 94, 171);

            float totalSpace = spacing * spacingVFac;


            buttonPanel.Append(searchButton);
            searchButton.OnClick += searchClick;
            searchButton.Left.Pixels = spacing;
            searchButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Append(optionsButton);
            optionsButton.OnClick += optionsClick;
            optionsButton.Left.Pixels = spacing;
            optionsButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            totalSpace += spacing * spacingVFac * 1.5f;

            buttonPanel.Append(configLoadButton);
            configLoadButton.OnClick += configLoadClick;
            configLoadButton.Left.Pixels = spacing;
            configLoadButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Append(configSaveButton);
            configSaveButton.OnClick += configSaveClick;
            configSaveButton.Left.Pixels = spacing;
            configSaveButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            totalSpace += spacing * spacingVFac * 1.5f;

            buttonPanel.Append(positiveButton);
            positiveButton.OnClick += positiveClick;
            positiveButton.Left.Pixels = spacing;
            positiveButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;


            buttonPanel.Append(clearButton);
            clearButton.OnClick += clearClick;
            clearButton.Left.Pixels = spacing;
            clearButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;



            totalSpace += spacing * spacingVFac * 2;

            buttonPanel.Append(stopButton);
            stopButton.OnClick += stopClick;
            stopButton.Left.Pixels = spacing;
            stopButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Height.Pixels = totalSpace;
            Append(buttonPanel);

            this.progressBar.MarginBottom = 70;
            this.progressBar.HAlign = 0.5f;
            this.progressBar.VAlign = 1f;
            this.progressBar.Recalculate();
            this.progressMessage.CopyStyle(this.progressBar);
            UIHeader expr_78_cp_0 = this.progressMessage;
            expr_78_cp_0.MarginBottom = 120f;
            this.progressMessage.Recalculate();
            this.progress = progress;
            base.Append(this.progressBar);
            base.Append(this.progressMessage);

            Init();
            currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);

        }

        

        private void Init()
        {
            optionList.Clear();
            infopanel.selectables.Clear();


            int mapSizeMain = Main.maxTilesY / 600 - 1;
            string mapSize = mapSizeMain == 1 ? "Small" : mapSizeMain == 2 ? "Medium" : mapSizeMain == 3 ? "Large" : "Unknown";
            string diff = Main.expertMode ? "Expert" : "Normal";
            string evilType = WorldGen.WorldGenParam_Evil == 1 ? "Crimnson" : WorldGen.WorldGenParam_Evil == 0 ? "Corruption" : "Random";

            addDictToInfo(OptionsDict.title).setCustomColor(Color.DarkOrange);


            addDictToInfo(OptionsDict.WorldInformation.worldSize).SetValue(mapSize);
            addDictToInfo(OptionsDict.WorldInformation.difficulty).SetValue(diff);
            addDictToInfo(OptionsDict.WorldInformation.evilType).SetValue(evilType);
            infopanel.AddTextInput(OptionsDict.Configuration.configName).SetValue("config");
            infopanel.AddTextInput(OptionsDict.WorldInformation.worldName).SetValue(Main.worldName);
            infopanel.AddTextInput(OptionsDict.Configuration.startingSeed).SetValue(Main.ActiveWorldFileData.Seed.ToString());
            addDictToInfo(OptionsDict.Configuration.searchSeedNum).SetValue("10000");
            //addDictToInfo(OptionsDict.GeneralOptions.searchRare).SetValue(opdict[OptionsDict.GeneralOptions.searchRare][opdict[OptionsDict.GeneralOptions.searchRare].Count - 1]);
            addSelectListToInfo(OptionsDict.GeneralOptions.omitRare, InfoPanel.listKindOmitRare);
            addSelectListToInfo(OptionsDict.GeneralOptions.naming, InfoPanel.listKindNaming);


            addFreeLine();
            addDictToInfo(OptionsDict.Phase1.title).setCustomColor(Color.Orange);
            addDictToInfo(OptionsDict.Phase1.pyramidsPossible).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.copperTin).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.ironLead).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.silverTungsten).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.goldPlatin).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.moonType).SetValue("Random");
            


            addFreeLine();
            addDictToInfo(OptionsDict.Phase2.title).setCustomColor(Color.Orange);
            addSelectListToInfo(OptionsDict.Phase2.positive, InfoPanel.listKindPositive);
            addSelectListToInfo(OptionsDict.Phase2.negative, InfoPanel.listKindNegative);
                         


            addFreeLine();
            addDictToInfo(OptionsDict.Phase3.title).setCustomColor(Color.Orange);
            addDictToInfo(OptionsDict.Phase3.continueEvaluation).SetValue("Start new");
            addSelectListToInfo(OptionsDict.Phase3.positive, InfoPanel.listKindPositive);
            addSelectListToInfo(OptionsDict.Phase3.negative, InfoPanel.listKindNegative);


            InitCountText();
            writtenText = "";

        }

        const int statsDimPy = 8;
        const int statsDimCou = 15;
        private void InitCountText(){
            
            countText = "##. 00";

            for (int j = 1; j <= statsDimPy; j++)
            {
                countText += "."+ j.ToString().PadLeft(2, '0');
            }
            countText += " #.+" + Environment.NewLine; // + "--------------------------\n";

            for (int i = 0; i <= statsDimCou; i++)
            {
                countText += i.ToString().PadLeft(2, '.')+"# 00";
                for (int j = 1; j <= statsDimPy; j++)
                    countText += ".00";
                countText += " #00"+Environment.NewLine;
            }
            //countText += "--------------------------\n";
            countText += ".+# 00";
            for (int j = 1; j <= statsDimPy; j++)
            {
                countText += ".00";
            }
            countText += " ?00";
        }

        public void SetCountText(int[,] hasCount, int[] chanceCount)
        {
            //todo, very very slow
            for (int c = 0; c <= statsDimCou; c++) {
                int num = 0;
                for (int p = 0; p <= statsDimPy; p++)
                {
                    num += hasCount[c, p];
                    ChangeCountText(hasCount, chanceCount, c, p);

                }
                ChangeCountText(hasCount, chanceCount, c);
            }







        }



        private Action<char[], int, int, int> GetSetPowFV = (chars_, chance_, numPyr_, value_) =>
        {
            int temp = 1;
            int c = -1;

            while (temp <= value_)
            {
                c++;
                temp *= 10;
            }
            if (c == -1) c++;
            char pow10 = (char)(c + '0');
            char fd = (char)((10 * value_) / temp + '0');

            int lineLl = (statsDimPy + 3) * 3 + 1 + Environment.NewLine.Length;
            int xoff = (numPyr_> statsDimPy ? 5:4);

            int pos = lineLl * (chance_ + 1) + xoff + 3 * numPyr_;
            chars_[pos] = pow10;
            chars_[pos + 1] = fd;
        };

        public void ChangeCountText(int[,] hasCount, int[] chanceCount, int chance)
        {
            if (chance > statsDimCou) return;


            char[] chars = countText.ToCharArray();

            int value = chanceCount[chance];

            for(int i=0; i <= statsDimPy; i++)            
                value += hasCount[chance, i];            

            GetSetPowFV(chars, chance, statsDimPy+ 1 , value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += chanceCount[i];

            GetSetPowFV(chars, statsDimCou +1, statsDimPy + 1, value);

            countText = new string(chars);

        }


        public void ChangeCountText(int[,] hasCount, int[] chanceCount, int chance, int numPyr)
        {
            if (chance > statsDimCou || numPyr > statsDimPy)
            {
                countText += "seed " + wgss.seed + " had a pyramid chance of " + chance + " and " + numPyr + " pyramids!"+ Environment.NewLine; 

                return;
            }


            char[] chars = countText.ToCharArray();
            
            int value = hasCount[chance, numPyr];
            GetSetPowFV(chars, chance, numPyr, value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += hasCount[i, numPyr];

            GetSetPowFV(chars, statsDimCou + 1, numPyr, value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += chanceCount[i];
            GetSetPowFV(chars, statsDimCou + 1, statsDimPy + 1, value);

            //WorldGenSeedSearch.writeDebugFile(" total chances " + value);
            countText = new string(chars); //tood faster                       

        }

        private void addFreeLine()
        {
            infopanel.AddSelectable("");
        }

        private SelectableText addDictToInfo(string what)
        {
            //bug avoid
            if (what.Equals(OptionsDict.WorldInformation.worldSize) && wgss.mediumWasDone && !wgss.largeWasDone)
                return infopanel.AddSelectable(what, opdict[OptionsDict.Bug.worldSizeBugMinMed]);
            else if (what.Equals(OptionsDict.WorldInformation.worldSize) && wgss.largeWasDone)
                return infopanel.AddSelectable(what, opdict[OptionsDict.Bug.worldSizeBugMinLarge]);

            return infopanel.AddSelectable(what, opdict[what]);
        }
        private SelectableText addSelectListToInfo(string what, char kind)
        {
            return infopanel.AddSelectableList(new SelectableList(what), kind);
        }

        
        public void HideUnhideProgressBar()
        {
            this.progressBar.MarginBottom = -this.progressBar.MarginBottom;
            this.Recalculate();
        }





        public bool rephrasing = false;
        int lastOptionSize = 0;

        public override void Update(GameTime gameTime)
        {
            if (detailsList != null && detailsPanel != null && detailsListScrollbar != null && !writeTextUpdating)
            {
                if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && (!wgss.inSearch || wgss.isInCreation) && (this.currentSize != (this.GetDimensions()).Width || writeText || ((detailsList.entryList.Count == 0) && writeStats) || (detailsList.entryList.Count == 0 && wgss.isInCreation && writtenText.Length > 0)))
                {
                    rephrasing = true;
                    writeText = true;
                    float currentSize = getDescListWith();
                    detailsList.fulltext = writtenStats + Environment.NewLine + writtenText; //if alawys true

                    detailsList.Rephrase(currentSize);
                    this.currentSize = (this.GetDimensions()).Width;
                    writeStats = false;
                    writeText = false;
                    rephrasing = false;
                }
                else
                if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && !wgss.inSearch && writeStats && !writeText && this.currentSize == (this.GetDimensions()).Width)
                {

                    //also throws erro
                    rephrasing = true;
                    writeText = true;
                    if (detailsList.entryList.Count > 0)
                        detailsList.entryList[0].SetTo(writtenStats);

                    writeStats = false;
                    writeText = false;
                    rephrasing = false;
                }


                if (infopanel != null && infopanel.uielem.Count != lastOptionSize)
                {

                    SetToConfiguration(Configuration.GenerateConfiguration(infopanel.selectables), false);

                    lastOptionSize = infopanel.uielem.Count;
                    //WorldGenSeedSearch.writeDebugFile(" updated size to " + lastOptionSize);
                }


            }
        }



        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            this.progressBar.SetProgress(this.progress.TotalProgress, this.progress.Value);
            this.progressMessage.Text = this.progress.Message;
            this.UpdateGamepadSquiggle();                        
           
        }

        private float getDescListWith()
        {
            //chances are high something wrong here
            return (detailsPanel.GetDimensions()).Width - (detailsListScrollbar.GetDimensions()).Width - detailsListScrollbar.MarginRight -
                detailsListScrollbar.MarginLeft - 
                detailsList.MarginRight - detailsList.PaddingRight - detailsList.PaddingLeft -detailsPanel.PaddingRight;
        }


        public void UpdateProgress(GenerationProgress progress)
        {
            this.progress = progress;
            this.progressBar.SetProgress(this.progress.TotalProgress, this.progress.Value);
            this.progressMessage.Text = this.progress.Message;
        }

        private void UpdateGamepadSquiggle()
        {
            Vector2 value = new Vector2((float)Math.Cos((double)(Main.GlobalTime * 6.28318548f)), (float)Math.Sin((double)(Main.GlobalTime * 6.28318548f * 2f))) * new Vector2(30f, 15f) + Vector2.UnitY * 20f;
            UILinkPointNavigator.Points[3000].Unlink();
            UILinkPointNavigator.SetPosition(3000, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value);
        }



        private void stopClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.inSearch)
            {
                wgss.ended = true;
                wgss.searchForSeed = false;
                wgss.stage = -1;
                
            }
        }


        private void SetBackToDefault()
        {
            //if (wgss.isInCreation || !wgss.inSearch)
            if (!wgss.searchForSeed)
            {
                Init();
                infopanel.selectables.Sort();
                currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
            }
        }


        double lastClear = 0;
        private void clearClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.isInCreation || wgss.searchForSeed)            
            {
                if (Main.GlobalTime - lastClear > 3 || wgss.searchForSeed)
                {
                    //reverse changes
                    if (currentConfig != null)
                        SetToConfiguration(currentConfig);

                    if (wgss.isInCreation)
                    {
                        while (writeText || writeStats) Thread.Sleep(30);
                        writtenText = "";
                        while (writeText || writeStats) Thread.Sleep(30);
                        writtenStats = "";
                        writeText = true;
                    }
                }
                else
                    SetBackToDefault();

                lastClear = Main.GlobalTime;
            }
        }



        private void searchClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed)
            {
                currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
                InitCountText();                
                wgss.searchForSeed = true;
                wgss.stage = 0;
                

            }
        }

        private void optionsClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.inSearch)
            {
                wgss.goToOptions(true); 
            }
        }


        private void configSaveClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed)
            {
                infopanel.selectables.Sort();

                Configuration config = Configuration.GenerateConfiguration(infopanel.selectables);

                config.SaveConfigFile(Main.SavePath + OptionsDict.Paths.configPath);
            }
        }

        private void configLoadClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed)
            {
                infopanel.selectables.Sort();
                Configuration config = Configuration.GenerateConfiguration(infopanel.selectables); //todo something faster to get name

                string configName = config.configName;

                if (configName.Length > 5 && configName.Substring(0, 6).Equals("config"))
                    configName = configName.Substring(6, configName.Length - ("config").Length);


                config = Configuration.LoadConfiguration(Main.SavePath + OptionsDict.Paths.configPath + "config" + configName + ".txt");

                if (config != null)
                    SetToConfiguration(config);
            }
        }

        int currentPositive = 0;
        private void positiveClick(UIMouseEvent evt, UIElement listeningElement)
        {
            //writeConfig();
            //WriteTextToDescList();

            if (!wgss.searchForSeed)
            {
                SetBackToDefault();
                string name = "config";
                if (currentPositive == 0)
                {
                    name = "Basics";
                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "3");

                    
                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");

                }
                else if (currentPositive == 1)
                {
                    name = "GoodSeed";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);


                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "10000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "4");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Tree Chest", "1");
                    currentConfig.InsertSelectableText(2,Configuration.ConfigItemType.SelectableListPositive, "Pyramid Carpet", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");
                    

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");

                }
                else if (currentPositive == 2)
                {
                    name = "ManyPyramid_JungleSnow";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Tree", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Pyramid", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Near Enchanted Sword", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Floating Island without chest", "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    string size = currentConfig.FindConfigItemValue("World size", 0);
                    int numPyramid = size.Equals("Large") ? 6 : size.Equals("Medium") ? 5 : size.Equals("Small") ? 4 : 7;
                    int numPyramidChance = size.Equals("Large") ? 9 : size.Equals("Medium") ? 7 : size.Equals("Small") ? 6 : 10;


                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "100000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, (numPyramidChance).ToString());

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Snow biome surface overlap mid", "10");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Jungle biome surface overlap mid", "10");


                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Number of Pyramids", numPyramid.ToString());

   
                }
                else if (currentPositive == 3)
                {
                    name = "VeryRare";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Tree", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Pyramid", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Near Enchanted Sword", "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "4");
                    currentConfig.ChangeValueOfSelectableText(3, Configuration.ConfigItemType.Header, OptionsDict.Phase3.continueEvaluation, OptionsDict.Phase3.continueEvaluatioTakeOverTag);


                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Snow biome surface overlap mid", "10");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Jungle biome surface overlap mid", "10");

                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Evil only one side", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Tree Chest Loom", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Carpet", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Has evil Ocean", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Ice surface more than half evil", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Tree to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Cloud to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Pyramid to mid", "800");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");

                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Staff of Regrowth", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Flower Boots", "1");

                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Temple Distance", "1500");                    
                }

                SetToConfiguration(currentConfig);
                Selectable configNameInput = infopanel.Search4ElementWithHeaderName(OptionsDict.Configuration.configName);
                configNameInput.SetValue(name);

                currentPositive++;
                if (currentPositive > 3) currentPositive = 0;
                writeText = true; 
            }            
        }



        public void WriteTextToDescList() { 
            if(writeTextUpdating) return;

            writeTextUpdating = true;
            detailsList.Clear();
            detailsList.UpdateText(writtenText);
            writeTextUpdating = false;
        }


        public void writeConfig()
        {
            infopanel.selectables.Sort();
            Configuration config = Configuration.GenerateConfiguration(infopanel.selectables); //todo something faster to get name

            string all = "";
            foreach (var cc in config.configList)
            {
                all += cc.name + "_ " + cc.value + "_ " + cc.phase + "_ " + cc.type + Environment.NewLine;
            }
            WorldGenSeedSearch.writeDebugFile(all);

        }




        public void SetToConfiguration(Configuration config, bool setToCurrent = true)
        {
            optionList.Clear();
                        

            infopanel.selectables.Clear();

            Configuration.ConfigItemType lastType = Configuration.ConfigItemType.Other;
            int stage = 0;
            bool inList = false;
            SelectableText lastAdd = null;
            foreach (Configuration.ConfigurationItem cci in config.configList)
            {
                if (cci.type == Configuration.ConfigItemType.SelectableListPositive && cci.type == lastType) continue;

                if ((lastType == Configuration.ConfigItemType.SelectableListPositive ||
                    lastType == Configuration.ConfigItemType.SelectableListNegative ||
                    lastType == Configuration.ConfigItemType.SelectableListName ||
                    lastType == Configuration.ConfigItemType.SelectableListOmitRare) &&
                    (cci.type == Configuration.ConfigItemType.SelectableText))
                        inList = true;
                if(cci.type != Configuration.ConfigItemType.SelectableText)
                        inList = false;

                if (cci.type == Configuration.ConfigItemType.SelectableListNegative && lastType != Configuration.ConfigItemType.SelectableListPositive)
                    addSelectListToInfo(stage == 2 ? OptionsDict.Phase2.positive : OptionsDict.Phase3.positive, InfoPanel.listKindPositive); //add another positve

                switch (cci.type)
                {
                    case Configuration.ConfigItemType.Title:
                        addDictToInfo(OptionsDict.title).setCustomColor(Color.DarkOrange);
                        break;
                    case Configuration.ConfigItemType.Header:
                        addDictToInfo(cci.name).setCustomColor(Color.Orange);
                        stage++;
                        break;
                    case Configuration.ConfigItemType.InputField:
                        infopanel.AddTextInput(cci.name).SetValue(cci.value);
                        break;
                    case Configuration.ConfigItemType.NewLine:
                        addFreeLine();
                        break;
                    case Configuration.ConfigItemType.SelectableText:
                        if (opdict.ContainsKey(cci.name))
                        {
                            //addDictToInfo(cci.name).SetValue(cci.value);
                            if (inList)
                                lastAdd.SelectOptionProp(cci.name).SetValue(cci.value);
                            else
                                addDictToInfo(cci.name).SetValue(cci.value);
                        }
                        //else
                            //infopanel.AddSelectable(cci.name, OptionsDict.vForUnknown); // <-- breaks formating
                        
                        break;
                        
                    case Configuration.ConfigItemType.SelectableListPositive:
                        lastAdd = addSelectListToInfo(stage== 2? OptionsDict.Phase2.positive : OptionsDict.Phase3.positive, InfoPanel.listKindPositive);
                        break;
                    case Configuration.ConfigItemType.SelectableListNegative:
                        lastAdd = addSelectListToInfo(stage == 2 ? OptionsDict.Phase2.negative : OptionsDict.Phase3.negative, InfoPanel.listKindNegative);                        
                        break;
                    case Configuration.ConfigItemType.SelectableListName:
                        lastAdd = addSelectListToInfo(OptionsDict.GeneralOptions.naming, InfoPanel.listKindNaming);
                        break;
                    case Configuration.ConfigItemType.SelectableListOmitRare:
                        lastAdd = addSelectListToInfo(OptionsDict.GeneralOptions.omitRare, InfoPanel.listKindOmitRare);
                        break;
                    case Configuration.ConfigItemType.Text:
                        infopanel.AddSelectable(cci.name);
                        break;
                }


                lastType = cci.type;
            }

            infopanel.selectables.Sort();
            if(setToCurrent)
                currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
            //infopanel.uielem.RecalculateChildren();
            //infopanel.uielem.Recalculate();



        }

    }

}