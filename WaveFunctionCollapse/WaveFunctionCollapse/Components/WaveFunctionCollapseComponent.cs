﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace WaveFunctionCollapse
{
    public partial class WaveFunctionCollapseComponent : GH_Component
    {
        public WaveFunctionCollapseComponent() : base("WaveFunctionCollapse", "WFC",
              "Me trying to code something", "TERM2", "WFC_WIP")
        {
       }

        // INPUT
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new PatternFromSampleParam());
            pManager.AddPointParameter("Wave", "", "", GH_ParamAccess.list);
        }

        // OUTPUT
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Number of rotated tiles", "Offsetes count", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Panel texts", "", "", GH_ParamAccess.list);
            pManager.AddParameter(new PatternHistoryParam());

        }

        int N = 2;
        // INSIDE
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //// WAVE TO OBSERVE: AREA FOR PATTERN
            List<Point3d> wavePoints = new List<Point3d>();
            DA.GetDataList<Point3d>(1, wavePoints);

            GH_PatternsFromSample gh_patterns = new GH_PatternsFromSample();
            DA.GetData<GH_PatternsFromSample>(0, ref gh_patterns);

            var patterns = gh_patterns.Value.Patterns;
            var tilesA = gh_patterns.Value.UnitElementsOfType0;
            var tilesB = gh_patterns.Value.UnitElementsOfType1;
            var allTiles = gh_patterns.Value.UnitElementsCenters;
            var weights = gh_patterns.Value.TilesWeights;

            // RUN WAVEFUNCION COLLAPSE
            var wfc = new WaveFunctionCollapseRunner();
            var history = wfc.Run(patterns, tilesA, tilesB, allTiles, N, wavePoints, weights);
            var return_value = new GH_WaveCollapseHistory(history);

            while (return_value == null) 
                {
                     wfc = new WaveFunctionCollapseRunner();
                     history = wfc.Run(patterns, tilesA, tilesB, allTiles, N, wavePoints, weights);
                     return_value = new GH_WaveCollapseHistory(history);
                }

            //var result = wfc.OutputObservations();
            //var red = wfc.OutputUnobserved();


            if (true)
            {
                DA.SetData(0, patterns.Count);
                DA.SetDataList(1, patterns.Select(p => p.ToString()));
                DA.SetData(2, return_value);

            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Check the inputs you idiot!");
            }
        }

 
        /// Provides an Icon for every component that will be visible in the User Interface. Icons need to be 24x24 pixels.
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return WaveFunctionCollapse.Properties.Resources.Icon;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3aac7ab0-722c-4eb0-b65a-e53640525e4b"); }

        }

        public object MyAssemblyName { get; private set; }
    }

    struct IntPoint3d
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }

}