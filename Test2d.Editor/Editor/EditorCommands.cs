﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class EditorCommands
    {
        /// <summary>
        /// 
        /// </summary>
        public ICommand NewCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand OpenCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand SaveAsCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExitCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportDataCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportDataCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand UpdateDataCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportStyleCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportStylesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportStyleLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportStyleLibrariesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportGroupCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportGroupsCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportGroupLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportGroupLibrariesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportTemplateCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportTemplatesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportStyleCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportStylesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportStyleLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportStyleLibrariesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportGroupCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportGroupsCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportGroupLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportGroupLibrariesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportTemplateCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportTemplatesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand UndoCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RedoCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand CopyAsEmfCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand CutCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand CopyCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand PasteCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand SelectAllCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ClearAllCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand GroupCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand GroupLayerCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolNoneCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolSelectionCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolGroupCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolPointCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolLineCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolRectangleCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolEllipseCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolArcCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolBezierCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolQBezierCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolTextCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolImageCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ToolPathCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand EvalCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand EvalScriptCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DefaultIsFilledCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand SnapToGridCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand TryToConnectCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddDatabaseCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddColumnCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveColumnCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddRecordCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ResetRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddBindingCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveBindingCommand { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ICommand AddPropertyCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemovePropertyCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddGroupLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddGroupCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddLayerCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddStyleLibraryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddStyleCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveShapeCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand StartSimulationCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand StopSimulationCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RestartSimulationCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand PauseSimulationCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand TickSimulationCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportShapeCodeCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportShapeCodeCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomResetCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomExtentCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DatabasesWindowCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand LayersWindowCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand StyleWindowCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand StylesWindowCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ShapesWindowCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand DocumentWindowCommand { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public ICommand ScriptWindowCommand { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public ICommand PropertiesWindowCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddTemplateCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveTemplateCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand EditTemplateCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ApplyTemplateCommand { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ICommand SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddContainerCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand InsertContainerBeforeCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand InsertContainerAfterCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddDocumentCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand InsertDocumentBeforeCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand InsertDocumentAfterCommand { get; set; }
    }
}