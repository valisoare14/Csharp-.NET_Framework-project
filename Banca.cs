using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Clienti_Banca
{
    [Serializable]
    class Aparat
    {
        int sold;
        public Aparat()
        {
            sold = 0;
        }
        public int Sold
        {
            get { return sold; }
            set {
                if (value >= 0)
                    sold = value;
                else
                    throw new Exception("Valoare Negativa");
            }
        }
    }
    [Serializable]
    class Client
    {
        string nume;
        Aparat aparat;
        public Client()
        {
            nume = "Anonim";
            aparat = new Aparat();
        }
        public string Nume
        {
            get { return nume; }
            set
            {
                nume= value;
            }
        }
        public int Sold
        {
            get { return aparat.Sold;}
            set
            {
                if(value>=0)
                    aparat.Sold = value;
            }
        }
        public void extragereNumerar(int valoareDeExtras)
        {
            if(valoareDeExtras >= 0)
            {
                if ((this.Sold - valoareDeExtras) >= 0)
                {
                    this.Sold=this.Sold-valoareDeExtras;
                }
                else
                {
                    throw new Exception("Solduri insuficiente!");
                }
            }
        }
        //returnare clone client
        public Client Clone()
        {
            return this.MemberwiseClone() as Client;    
        }
    }
    class Banca_eventArgs : EventArgs
    {
        int index;
        public Banca_eventArgs(int index)
        {
            this.index = index;
        }
        public int Index => index;
    }
    internal class Banca : ICloneable
    {
        //campuri banca:
        public event EventHandler Eveniment_Actualizare;
        List<Client> clienti;
        public Banca()
        {
            clienti = new List<Client>();
        }
        public Banca(Banca banca) { 
            clienti=new List<Client>();
            foreach(Client client in banca.clienti)
            {
                clienti.Add(client.Clone());
            }
        }
        public void Adauga(Client client)
        {
            clienti.Add(client);
            if(Eveniment_Actualizare != null)
            {
                Eveniment_Actualizare(this, new Banca_eventArgs(clienti.Count - 1));
            }
        }
        public void StergeClient(int index)
        {
            clienti.Remove(clienti[index]);
            if (Eveniment_Actualizare != null)
            {
                Eveniment_Actualizare(this, new Banca_eventArgs(clienti.Count - 1));
            }
        }
        public void RetragereSold(int sold_de_retras , int index)
        {
            clienti[index].Sold-=sold_de_retras;
            if(Eveniment_Actualizare != null)
            {
                Eveniment_Actualizare(this, new Banca_eventArgs(this.clienti.Count() - 1));
            }
        }
        public void DepunereSold(int sold_de_depus , int index)
        {
            clienti[index].Sold+=sold_de_depus;
            if (Eveniment_Actualizare != null)
            {
                Eveniment_Actualizare(this, new Banca_eventArgs(this.clienti.Count() - 1));
            }
        }
        public object Clone()
        {
            return new Banca(this);
        }
        public Client this[int k]//INDEXER
        {
            get=>k<clienti.Count ? clienti[k] : throw new IndexOutOfRangeException();
            set
            {
                if (k >= clienti.Count) throw new IndexOutOfRangeException();
                if (clienti[k] != value)
                {
                    clienti[k] = value;
                    if(Eveniment_Actualizare != null)
                    {
                        Eveniment_Actualizare(this, new Banca_eventArgs(k));
                    }
                }
            }
        }
        public int Capitaluri_Banca => clienti.Sum(x => x.Sold);
        public int NumarClienti=>clienti.Count;

        public void Serializare(string file)
        {
            FileStream filestream = new FileStream(file, FileMode.Create);
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            binaryFormatter.Serialize(filestream, clienti);
            filestream.Close();
        }
        public void DeSerizalizare(string file)
        {
            FileStream filestream = new FileStream(file, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            clienti=binaryFormatter.Deserialize(filestream) as List<Client>;
            filestream.Close();
            if(Eveniment_Actualizare != null)
            {
                Eveniment_Actualizare(this, new Banca_eventArgs(clienti.Count - 1));
            }
        }
    }
}
