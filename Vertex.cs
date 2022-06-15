/*
Copyright (C) Deepali Srivastava - All Rights Reserved
This code is part of DSA course available on CourseGalaxy.com    
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace WindowsFormsApplication35
{
    class Vertex
    {
        public String name;
        public int status;
        public int predecessor;
        public int pathLength;
        
        public Vertex(String name)
        {
            this.name = name;
        }
    }
}