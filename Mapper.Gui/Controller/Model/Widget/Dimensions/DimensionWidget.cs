﻿using Mapper.Gui.Logic;
using Mapper.Gui.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using WorldEditor;

namespace Mapper.Gui.Controller
{
    public class DimensionWidget : IDimensionWidget
    {
        public IReadOnlyList<DimensionUI> Dimensions => DimensionsWriteable;
        public List<DimensionUI> DimensionsWriteable { get; set; } = new List<DimensionUI>();

        public IReadOnlyList<Dimension> ExtraDimensions => ExtraDimensionsWriteable;
        public List<Dimension> ExtraDimensionsWriteable { get; set; } = new List<Dimension>();

        public Dimension CurrentDimension
        {
            get
            {
                if (Scene.Domain.CurrentWorld is null) return new Dimension(string.Empty);
                return Scene.Domain.CurrentWorld.CurrentDimension.Dimension;
            }
            set
            {
                if (Scene.Domain.CurrentWorld is null) return;

                Scene.ChangeDimension(value);
                DimensionSelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? DimensionUpdate;
        public event EventHandler? DimensionSelectionChanged;

        public Scene Scene { get; }

        public DimensionWidget(Scene scene) 
        {
            DimensionsWriteable.Add(new DimensionUI(Dimension.Overworld, new BitmapImage(new Uri("/Resources/Image/Dimension/Overworld_16px.png", UriKind.Relative))));
            DimensionsWriteable.Add(new DimensionUI(Dimension.Nether, new BitmapImage(new Uri("/Resources/Image/Dimension/TheNether_16px.png", UriKind.Relative))));
            DimensionsWriteable.Add(new DimensionUI(Dimension.TheEnd, new BitmapImage(new Uri("/Resources/Image/Dimension/TheEnd_16px.png", UriKind.Relative))));

            Scene = scene;
            Scene.WorldChanged += Scene_WorldChanged;
        }

        private void Scene_WorldChanged(WorldDomain? old, WorldDomain current) 
        {
            SetExtraDimensions();
            DimensionUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void SetExtraDimensions() 
        {
            ExtraDimensionsWriteable.Clear();

            if (Scene.Domain.CurrentWorld is null) return;
            string directory = $"{Scene.Domain.CurrentWorld.Level.Directory}\\dimensions";

            if (!Directory.Exists(directory)) return;

            string[] namespaces = Directory.GetDirectories(directory);
            foreach (string @namespace in namespaces) 
            {
                string[] dimensions = Directory.GetDirectories(@namespace);
                foreach (string dimension in dimensions) 
                {
                    ExtraDimensionsWriteable.Add(new Dimension($"{Path.GetFileName(@namespace)}:{Path.GetFileName(dimension)}"));
                }
            }
        }

        public bool IsDimensionAllowed(Dimension dimension)
        {
            return Scene.Domain.CurrentWorld is not null;
        }
    }
}
