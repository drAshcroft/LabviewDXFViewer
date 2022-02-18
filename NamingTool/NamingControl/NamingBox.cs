using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamingControl
{
    public delegate void FilenameUpdatedEvent(string ivFilename, string rtFilename);
    public partial class NamingBox : UserControl
    {
        public event FilenameUpdatedEvent onFilenameUpdated;
        public NamingBox()
        {
            InitializeComponent();
        }
        public string Filename_IV { get; set; }
        public string Filename_RT { get; set; }

        private NamingParameters _NamingInfo;
        public NamingParameters NamingInfos
        {
            get
            {
                if (_NamingInfo == null)
                    _NamingInfo = new NamingParameters();

                _NamingInfo.WaferChip = tbWaferChip.Text;
                _NamingInfo.Tags = tbTags.Text;
                _NamingInfo.Control = tbControl.Text;
                _NamingInfo.Experiment = tbExperiment.Text;
                _NamingInfo.Buffer = tbBuffer.Text;
                _NamingInfo.Notes = tbNotes.Text;
                _NamingInfo.BiasVoltage = tbBias.Text;
                _NamingInfo.ReferenceVoltage = tbReference.Text;
                _NamingInfo.ChannelNamesTB = tbChannelNames.Text;
                _NamingInfo.ChannelNames = ChannelNames;
                return _NamingInfo;
            }
            set
            {
                _NamingInfo = value;
                if (_NamingInfo != null)
                {
                    tbWaferChip.Text = _NamingInfo.WaferChip;
                    tbTags.Text = _NamingInfo.Tags;
                    tbControl.Text = _NamingInfo.Control;
                    tbExperiment.Text = _NamingInfo.Experiment;
                    tbBuffer.Text = _NamingInfo.Buffer;
                    tbNotes.Text = _NamingInfo.Notes;
                    tbBias.Text = _NamingInfo.BiasVoltage;
                    tbReference.Text = _NamingInfo.ReferenceVoltage;
                    tbChannelNames.Text = _NamingInfo.ChannelNamesTB;
                    ChannelNames = _NamingInfo.ChannelNames;
                    UpdateFileName();
                }
            }
        }

        public string BaseDirectory { get; set; } = @"C:\DATASTORE";

        public string ChannelNames { get; set; } = "";

        public FileNamer FileBoxIV { get; set; }
        public FileNamer FileBoxRT { get; set; }

        public void UpdateFileName()
        {
            var waferChip = tbWaferChip.Text.Trim().ToUpper().Split(new string[] { ",", "\\", "/", "-", " " }, StringSplitOptions.RemoveEmptyEntries);
            var Tags = tbTags.Text.Trim().ToUpper().Split(new string[] { ",", "\\", "/", "-", " " }, StringSplitOptions.RemoveEmptyEntries);
            var control = tbControl.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
                .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var experiment = tbExperiment.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
                .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var buffer = tbBuffer.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
                .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var reference = tbReference.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
               .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var bias = tbBias.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
              .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var notes = tbNotes.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_")
                .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");
            var channelNames = "";

            if (ChannelNames != null)
                channelNames = ChannelNames.Trim().Replace(" ", ",").Replace("/", ",").Replace("-", ",")
                    .Replace("\\", ",");

            string fileNameRT = BaseDirectory + "\\" + string.Join("\\", waferChip) + "\\";
            string fileNameIV = BaseDirectory + "\\" + string.Join("\\", waferChip) + "\\";

            if (tbTags.Text.Trim() != "")
            {
                fileNameRT += string.Join("\\", Tags) + "\\";
                fileNameIV += string.Join("\\", Tags) + "\\";
            }

            fileNameRT += channelNames.Replace(",", "") + "_";
            fileNameRT += "_E-" + experiment;
            if (notes != "") fileNameRT += "_N-" + notes;
            if (buffer != "") fileNameRT += "_B-" + buffer;
            if (bias != "") fileNameRT += "_V-" + bias;
            if (reference != "") fileNameRT += "_R-" + reference;
            if (control != "") fileNameRT += "_C-" + control;


            fileNameIV += channelNames.Replace(",", "") + "_";
            fileNameIV += "_E-" + experiment;
            if (notes != "") fileNameIV += "_N-" + notes;
            if (buffer != "") fileNameIV += "_B-" + buffer;
            if (reference != "") fileNameIV += "_R-" + reference;

            Filename_IV = fileNameIV + "_IV";
            Filename_RT = fileNameRT + "_RT";

            if (FileBoxIV != null)
                FileBoxIV.SetFilename(Filename_IV);
            if (FileBoxRT != null)
                FileBoxRT.SetFilename(Filename_RT);


            onFilenameUpdated?.Invoke(Filename_IV, Filename_RT);
        }

        public DisplayMode DisplayMode
        {
            get;
            set;
        }

        #region TextChanges


        private void tbControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFileName();
        }

        private void tbNotes_TextChanged(object sender, EventArgs e)
        {
            UpdateFileName();
        }





        private void tbWaferChip_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateFileName();
        }

        private void tbTags_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateFileName();
        }

        private void tbControl_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateFileName();
        }

        private void tbChannelNames_KeyUp(object sender, KeyEventArgs e)
        {
            ChannelNames = tbChannelNames.Text;
            UpdateFileName();
        }


        #endregion

        public string[] Channels
        {
            get
            {
                var channels = tbChannelNames.Text.ToLower().Trim();
                if (channels == "device15")
                    channels = "W5,N1,N5,E1,E5,S1,S5,W1";
                if (channels == "device26")
                    channels = "W6,N2,N6,E2,E6,S2,S6,W2";
                if (channels == "device37")
                    channels = "W7,N3,N7,E3,E7,S3,S7,W3";
                if (channels == "device48")
                    channels = "W8,N4,N8,E4,E8,S4,S8,W4";
                return channels.ToUpper().Split(new string[] { ",", " ", "\\", "/", "-", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] ServerTags
        {
            get
            {
                var wafers = tbWaferChip.Text.Trim().ToUpper().Split(new string[] { ",", " ", "\\", "/", "-", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var tags = tbTags.Text.Trim().ToUpper().Split(new string[] { ",", " ", "\\", "/", "-", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                var experiment = tbExperiment.Text.Trim().Replace(" ", "-").Replace("_", "-").Replace("\\", "_").Replace("\n", "").Replace("\r", "")
               .Replace("/", "_").Replace(".", "-").Replace(":", "-").Replace(",", "_");

                wafers.AddRange(tags);
                wafers.Add(experiment);
                return wafers.ToArray();
            }
        }
        public string[] TDMS_PropertyValues
        {
            get
            {
                return new string[] { tbWaferChip.Text,tbTags.Text.Trim(), tbControl.Text.Trim(), tbExperiment.Text, tbBuffer.Text.Trim(),
                tbReference.Text.Trim(), tbBias.Text.Trim(), tbNotes.Text.Trim(), ChannelNames.Trim() };
            }
        }
        public string[] TDMS_PropertyNames
        {
            get
            {
                return new string[] { "waferChip", "Tags", "control", "experiment", "buffer", "reference", "bias", "notes", "channelNames" };
            }
        }
        #region ButtonClicks
        private void b15_Click(object sender, EventArgs e)
        {
            ChannelNames = "Device15";
            tbChannelNames.Text = "W5,N1,N5,E1,E5,S1,S5,W1";


            UpdateFileName();

        }

        private void b26_Click(object sender, EventArgs e)
        {
            ChannelNames = "Device26";
            tbChannelNames.Text = "W6,N2,N6,E2,E6,S2,S6,W2";
            UpdateFileName();
        }

        private void b37_Click(object sender, EventArgs e)
        {
            ChannelNames = "Device37";
            tbChannelNames.Text = "W7,N3,N7,E3,E7,S3,S7,W3";
            UpdateFileName();
        }

        private void b48_Click(object sender, EventArgs e)
        {
            ChannelNames = "Device48";
            tbChannelNames.Text = "W8,N4,N8,E4,E8,S4,S8,W4";
            UpdateFileName();
        }

        private void bNextStep_Click(object sender, EventArgs e)
        {
            tbControl.Text = tbExperiment.Text;
            SetCompletes();
            tbExperiment.Text = "";
            UpdateFileName();
        }

        public void SetCompletes()
        {
            if (tbExperiment.Text != "")
            {
                try
                {
                    if (aOptions.Contains(tbExperiment.Text) == false)
                    {
                        aOptions.Add(tbExperiment.Text);

                        tbControl.Items.Add(tbExperiment.Text);
                        tbExperiment.Items.Add(tbExperiment.Text);

                        tbControl.AutoCompleteCustomSource.Add(tbExperiment.Text);
                        tbExperiment.AutoCompleteCustomSource.Add(tbExperiment.Text);


                        var newOptions = "";
                        foreach (var x in aOptions)
                            newOptions += x.ToString() + "\n";
                        newOptions = newOptions.Trim();

                        File.WriteAllText(BaseDirectory + "\\Analytes.txt", newOptions);
                    }
                }
                catch { }
                try
                {
                    if (buffOptions.Contains(tbBuffer.Text) == false)
                    {
                        buffOptions.Add(tbBuffer.Text);

                        tbBuffer.Items.Add(tbBuffer.Text);
                        tbBuffer.AutoCompleteCustomSource.Add(tbBuffer.Text);


                        var newOptions = "";
                        foreach (var x in buffOptions)
                            newOptions += x.ToString() + "\n";
                        newOptions = newOptions.Trim();

                        File.WriteAllText(BaseDirectory + "\\Buffers.txt", newOptions);
                    }
                }
                catch { }
            }
        }



        #endregion

        AutoCompleteStringCollection aOptions = new AutoCompleteStringCollection();
        AutoCompleteStringCollection buffOptions = new AutoCompleteStringCollection();
        private void NamingBox_Load(object sender, EventArgs e)
        {
            string optionFile = BaseDirectory + "\\Analytes.txt";
            if (File.Exists(optionFile))
            {
                try
                {
                    var options = File.ReadAllText(optionFile).Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                    aOptions.AddRange(options.Distinct().ToArray());
                }
                catch
                {
                    aOptions.Add("1mM_PB");
                    aOptions.Add("IgE");
                    aOptions.Add("AuNP");
                }
            }
            else
            {
                aOptions.Add("1mM_PB");
                aOptions.Add("IgE");
                aOptions.Add("AuNP");
            }

            string optionFileBuff = BaseDirectory + "\\Buffers.txt";
            if (File.Exists(optionFileBuff))
            {
                try
                {
                    var options = File.ReadAllText(optionFileBuff).Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                    buffOptions.AddRange(options.Distinct().ToArray());
                }
                catch
                {
                    buffOptions.Add("1mM_PB");
                    buffOptions.Add("DIW");
                }
            }
            else
            {
                buffOptions.Add("1mM_PB");
                buffOptions.Add("DIW");
            }


            ConvertAutoComplete(aOptions, tbControl);
            ConvertAutoComplete(aOptions, tbExperiment);

            ConvertAutoComplete(buffOptions, tbBuffer);

        }

        private void ConvertAutoComplete(AutoCompleteStringCollection list, ComboBox box)
        {
            string[] a = new string[list.Count];
            for (var i = 0; i < list.Count; i++)
                a[i] = (list[i].ToString());


            box.AutoCompleteCustomSource = aOptions;
            box.Items.AddRange(a.ToArray());
        }
    }

    public class NamingParameters
    {

        public string WaferChip { get; set; }
        public string Tags { get; set; }
        public string Control { get; set; }
        public string Experiment { get; set; }
        public string Buffer { get; set; }
        public string Notes { get; set; }
        public string BiasVoltage { get; set; }
        public string ReferenceVoltage { get; set; }
        public string ChannelNames { get; set; }
        public string ChannelNamesTB { get; set; }
    }

    public enum DisplayMode
    {
        IV, RT
    }
}
