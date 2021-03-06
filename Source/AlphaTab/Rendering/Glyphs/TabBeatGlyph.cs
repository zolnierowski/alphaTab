﻿/*
 * This file is part of alphaTab.
 * Copyright (c) 2014, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */
using AlphaTab.Model;
using AlphaTab.Rendering.Utils;

namespace AlphaTab.Rendering.Glyphs
{
    public class TabBeatGlyph : BeatOnNoteGlyphBase
    {
        public TabNoteChordGlyph NoteNumbers { get; set; }
        public TabRestGlyph RestGlyph { get; set; }

        public override void DoLayout()
        {
            // create glyphs
            if (!Container.Beat.IsRest)
            {
                //
                // Note numbers
                NoteNumbers = new TabNoteChordGlyph(0, 0, Container.Beat.GraceType != GraceType.None);
                NoteNumbers.Beat = Container.Beat;
                NoteNumbers.BeamingHelper = BeamingHelper;
                NoteLoop(CreateNoteGlyph);
                AddGlyph(NoteNumbers);

                //
                // Whammy Bar
                if (Container.Beat.HasWhammyBar && !NoteNumbers.BeatEffects.ContainsKey("Whammy"))
                {
                    NoteNumbers.BeatEffects["Whammy"] = new WhammyBarGlyph(Container.Beat, Container);
                }

                //
                // Tremolo Picking
                if (Container.Beat.IsTremolo && !NoteNumbers.BeatEffects.ContainsKey("Tremolo"))
                {
                    NoteNumbers.BeatEffects["Tremolo"] = new TremoloPickingGlyph(5 * Scale, 0,
                        Container.Beat.TremoloSpeed.Value);
                }
            }
            else
            {
                RestGlyph = new TabRestGlyph();
                RestGlyph.Beat = Container.Beat;
                RestGlyph.BeamingHelper = BeamingHelper;
                AddGlyph(RestGlyph);
            }

            // left to right layout
            if (Glyphs == null) return;
            var w = 0f;
            for (int i = 0, j = Glyphs.Count; i < j; i++)
            {
                var g = Glyphs[i];
                g.X = w;
                g.Renderer = Renderer;
                g.DoLayout();
                w += g.Width;
            }
            Width = w;
        }

        public override void UpdateBeamingHelper()
        {
            if (!Container.Beat.IsRest)
            {
                NoteNumbers.UpdateBeamingHelper(Container.X + X);
            }
            else
            {
                RestGlyph.UpdateBeamingHelper(Container.X + X);
            }
        }

        private void CreateNoteGlyph(Note n)
        {
            var tr = (TabBarRenderer)Renderer;
            var noteNumberGlyph = new NoteNumberGlyph(0, 0, n);
            var l = n.Beat.Voice.Bar.Staff.Track.Tuning.Length - n.String + 1;
            noteNumberGlyph.Y = tr.GetTabY(l, -2);
            noteNumberGlyph.Renderer = Renderer;
            noteNumberGlyph.DoLayout();
            NoteNumbers.AddNoteGlyph(noteNumberGlyph, n);
        }
    }
}
