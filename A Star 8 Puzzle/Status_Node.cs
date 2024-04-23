using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace A_Star_8_Puzzle
{
	public class Status_Node
	{
		private Status_Node m_Parent;
		/*					 2 0 3
							 1 8 4
							 7 6 5
		 */
		private string m_Code;
		
		private char[,] m_Map = new char[3, 3];
		private int m_G; 	
		private int m_H;
		private Font m_Font;
		
		public string Code
		{
			get {return m_Code;}
			set
			{
				m_Code = value;
				for(int i = 0; i < 9; i++)
				{
					m_Map[i / 3,  i % 3] = value[i]; 
				}
			}
		}
		
		public Status_Node Parent
		{
			get {return m_Parent;}
			set {m_Parent = value;}
		}
		
		public char[,] Map
		{
			get {return m_Map;}
			set 
			{
				m_Map = value;
				StringBuilder sb = new StringBuilder(9);
				
				for(int i = 0; i < 3; i++)
				{
					for(int j = 0; j < 3; j++)
					{
						sb.Append(value[i, j]);
					}
				}
				m_Code = sb.ToString();
			}
		}
		
		public int F
		{
			get {return m_G + m_H;}
		}

        public int G
        {
            get {return m_G;}
            set {m_G = value;}
        }

        public int H
        {
            get {return m_H;}
            set {m_H = value;}
        }
		
		public Status_Node(string code, Status_Node parent, int g, int h)
		{
			Code = code;
            m_Parent = parent;
            m_G = g;
            m_H = h;
            m_Font = new Font("Candara", 25F, FontStyle.Regular, GraphicsUnit.Point);
		}

        public void Paint (Graphics g) 
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if(m_Map[i, j] != '0')
                    {
                        g.FillRectangle(Brushes.DarkCyan, 96*j, 96*i, 95, 95);
                        g.DrawString(m_Map[i, j].ToString(), m_Font, Brushes.White, 95*j + 26, 95*i + 26);
                    }
                }
            }
        }
	}
}
