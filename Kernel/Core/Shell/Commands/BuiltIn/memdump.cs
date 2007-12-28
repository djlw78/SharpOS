﻿

using System;
using System.Collections.Generic;
using System.Text;
using SharpOS.AOT.Attributes;
using SharpOS.Foundation;
using SharpOS.ADC;

namespace SharpOS.Shell.Commands.BuiltIn
{
    public unsafe static class memdump
    {
        public const string name = "memdump";
        public const string shortDescription = "Restarts the machine.";
        public const string lblExecute = "COMMANDS.memdump.Execute";
        public const string lblGetHelp = "COMMANDS.memdump.GetHelp";

        [Label( lblExecute )]
        public static void Execute( CommandExecutionContext* context )
        {
            ADC.MemoryManager.Dump( );
        }

        [Label( lblGetHelp )]
        public static void GetHelp( CommandExecutionContext* context )
        {
            TextMode.WriteLine( "Syntax: " );
            TextMode.WriteLine( "     memdump" );
            TextMode.WriteLine( "" );
            TextMode.WriteLine( "Displays memory usage." );
        }

        public static CommandTableEntry* CREATE( )
        {
            CommandTableEntry* entry = (CommandTableEntry*) SharpOS.ADC.MemoryManager.Allocate( (uint) sizeof( CommandTableEntry ) );

            entry->name = (CString8*) SharpOS.Stubs.CString( name );
            entry->shortDescription = (CString8*) SharpOS.Stubs.CString( shortDescription );
            entry->func_Execute = (void*) SharpOS.Stubs.GetLabelAddress( lblExecute );
            entry->func_GetHelp = (void*) SharpOS.Stubs.GetLabelAddress( lblGetHelp );
            entry->nextEntry = null;

            return entry;
        }
    }
}