using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuttyManager
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "PuttyManager")]
    public partial class ConfigManager
    {

        private List<PuttyManagerProfile> profilesField;

        public bool containsName(string name)
        {
            foreach (PuttyManagerProfile p in profilesField)
            {
                if (p.name.ToLower() == name.ToLower()) return true;
            }
            return false;
        }

        private string versionField;
        private string filezillapathField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("profile", IsNullable = false)]
        public List<PuttyManagerProfile> profiles
        {
            get
            {
                return this.profilesField;
            }
            set
            {
                this.profilesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string filezillapath
        {
            get
            {
                return this.filezillapathField;
            }
            set
            {
                this.filezillapathField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PuttyManagerProfile
    {

        private string nameField;

        private string hostnameField;

        private string userField;

        private string passField;

        private string commentField;

        private string scriptField;

        private bool useScriptField;

        private int portField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string hostname
        {
            get
            {
                return this.hostnameField;
            }
            set
            {
                this.hostnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get
            {
                return this.userField;
            }
            set
            {
                this.userField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pass
        {
            get
            {
                return this.passField;
            }
            set
            {
                this.passField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string script
        {
            get
            {
                return this.scriptField;
            }
            set
            {
                this.scriptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool useScript
        {
            get
            {
                return this.useScriptField;
            }
            set
            {
                this.useScriptField = value;
            }
        }
    }

}
