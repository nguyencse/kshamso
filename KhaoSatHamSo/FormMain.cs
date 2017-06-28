using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KhaoSatHamSo
{
    public partial class FormMain : Form
    {
        private string path;

        public FormMain()
        {
            InitializeComponent();
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnKS_Click(object sender, EventArgs e)
        {
            // Tạo file solve.txt chứa các câu lệnh maple để giải quyết bài toán khảo sát hàm số
            File.WriteAllText(path + "/packages/solve.txt", File.ReadAllText(path + "/packages/KSHS.txt"));

            string[] query =
            {
                "with(KhaoSatHamSo);",
                "f:=" + txtInput.Text + ";",
                "latex(f,\"f.txt\");",
                "latex([TapXacDinh(f)],\"txd.txt\");",
                "latex([DiemCucDai(f)],\"dcd.txt\");",
                "latex([DiemCucTieu(f)],\"dct.txt\");",
                "latex([KhoangBienThien(f)],\"kbt.txt\");",
                "latex([KhoangLoiLom(f)],\"kll.txt\");",
                "latex([TapDiemUon(f)],\"tdu.txt\");",
                "latex([TiemCanDung(f)],\"tcd.txt\");",
                "latex([TiemCanXien(f)],\"tcx.txt\");",
                "plotsetup(gif, plotoutput=`graph.gif`,plotoptions=`width=399,height=387`);",
                "VeDoThi(f);"
        };

            File.AppendAllLines(path + "/packages/solve.txt", query);

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = path + "/packages/solve.bat";
            processInfo.WorkingDirectory = Path.GetFullPath(path + "/packages");
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            Process process = Process.Start(processInfo);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format("`maple/solve.bat` exit with code = {0}", process.ExitCode));
            }

            CreateEquation(path + "/packages/txd.txt", path + "/packages/txd.gif", picTXD);
            CreateEquation(path + "/packages/dcd.txt", path + "/packages/dcd.gif", picDCD);
            CreateEquation(path + "/packages/dct.txt", path + "/packages/dct.gif", picDCT);
            CreateEquation(path + "/packages/kbt.txt", path + "/packages/kbt.gif", picKBT);
            CreateEquation(path + "/packages/kll.txt", path + "/packages/kll.gif", picKLL);
            CreateEquation(path + "/packages/tdu.txt", path + "/packages/tdu.gif", picTDU);
            CreateEquation(path + "/packages/tcd.txt", path + "/packages/tcd.gif", picTCD);
            CreateEquation(path + "/packages/tcx.txt", path + "/packages/tcx.gif", picTCX);

            if (picDT.Image != null)
            {
                picDT.Image.Dispose();
            }
            picDT.Image = Image.FromFile(path + "/packages/graph.gif");
        }

        private void CreateEquation(string fullLatexFilePath, string fullGifImagePath, PictureBox pic)
        {
            try
            {
                string equation = File.ReadAllText(fullLatexFilePath);
                if (equation.Length > 0)
                {
                    if (pic.Image != null)
                    {
                        pic.Image.Dispose();
                    }
                    NativeMethods.CreateGifFromEq(equation, fullGifImagePath);
                    pic.Image = Image.FromFile(fullGifImagePath);
                }
                else
                {
                    pic.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = dialog_1.ShowDialog();
            if (dialog_result == DialogResult.OK)
            {
                string pathCMaple = dialog_1.SelectedPath;
                File.WriteAllText(path + "/packages/solve.bat", "\"" + pathCMaple + "/cmaple.exe\" solve.txt");
                btnKS.Enabled = true;
            }
        }
    }
}
