using System;
using System.Collections.Generic;
using System.Linq;

namespace hw1_family_ties2
{
    class Program
    {
        static void Main(string[] args)
        {
            Person Natasha = new Person("Natasha", Gender.Female);
            Person Gleb = new Person("Gleb", Gender.Male);
            Natasha.SetPartner(Gleb);

            Person Vera = new Person("Vera", Gender.Female);
            Person Andrey = new Person("Andrey", Gender.Male);
            Vera.SetPartner(Andrey);

            Person Katya = new Person("Katya", Gender.Female, Natasha, Gleb);
            Person Pasha = new Person("Pasha", Gender.Male, Andrey, Vera, Katya);

            Person Egor = new Person("Egor", Gender.Female, Katya, Pasha);

            Person Nastia = new Person("Nastia", Gender.Female, Andrey, Vera);
            Person Sasha = new Person("Sasha", Gender.Male, Andrey, Vera);

            Person Roman = new Person("Roman", Gender.Male);
            Roman.SetPartner(Nastia);
            Person Misha = new Person("Misha", Gender.Male, Roman, Nastia);

            Console.WriteLine("Person:" + Egor.GetName());
            Console.WriteLine("Родители:");
            Egor.PrintParents();
            Console.WriteLine('\n');

            Console.WriteLine("Person:" + Natasha.GetName());
            Console.WriteLine("Родители:");
            Natasha.PrintParents();
            Console.WriteLine('\n');

            Console.WriteLine("Person:" + Egor.GetName());
            Console.WriteLine("Дяди и тёти:");
            foreach (Person person in Egor.GetUncles())
            {
                Console.WriteLine(person.GetName());
            }

            Console.WriteLine('\n');

            Console.WriteLine("Person:" + Egor.GetName());
            Console.WriteLine("Сёстры и братья:");
            foreach (Person person in Egor.GetCousins())
            {
                Console.WriteLine(person.GetName());
            }
            Console.WriteLine('\n');

            Console.WriteLine("Person:" + Pasha.GetName());
            Console.WriteLine("Родня со стороны мужа жены:");
            foreach (Person person in Pasha.GetInLaws())
            {
                Console.WriteLine(person.GetName());
            }

            var person1 = new Person("Person 1", Gender.Male);
            var person2 = new Person("Person 2", Gender.Female);
            var person3 = new Person("Person 3", Gender.Female);
            var person4 = new Person("Person 4", Gender.Male);
            person1.SetChildren(person2);
            person2.SetChildren(person3);
            person3.SetChildren(person1);
            person1.SetSingleParent(person4);
            person1.PrintParents();
        }
    }

    enum Gender
    {
        Female,
        Male
    }

    class Person
    {
        private string Name;
        private Gender Gender;
        private Person[] Parents = new Person[2];
        private Person Partner;
        private HashSet<Person> Childrens = new HashSet<Person>();

        public Person(string name, Gender gender)
        {
            this.Name = name;
            this.Gender = gender;
        }

        public Person(string name, Gender gender, Person parent_1, Person parent_2)
        {
            this.Name = name;
            this.Gender = gender;
            this.SetParents(parent_1, parent_2);
            parent_1.SetChildren(this);
        }

        public Person(string name, Gender gender, Person parent_1, Person parent_2, Person partner)
        {
            this.Name = name;
            this.Gender = gender;
            this.SetParents(parent_1, parent_2);
            this.SetPartner(partner);
            parent_1.SetChildren(this);
        }

        public void SetParents(Person parent_1, Person parent_2)
        {
            if (this.Parents[0] == null && this.Parents[1] == null)
            {
                if (this != parent_1 && this != parent_2 && parent_1 != parent_2 && parent_1.Partner == parent_2 && parent_2.Partner == parent_1)
                {
                    var existParents = GetUpTree(this.Parents);
                    var existChilds = GetDownTree(this.Childrens);
                    if (!existParents.Contains(parent_1) && !existParents.Contains(parent_2) && !existChilds.Contains(parent_1) && !existChilds.Contains(parent_2))
                    {
                        this.Parents[0] = parent_1;
                        this.Parents[1] = parent_2;
                        parent_1.SetChildren(this);
                        parent_2.SetChildren(this);
                    }
                }
            }
        }

        public void SetSingleParent(Person parent)
        {
            if (this == parent) return;
            if (this.Parents[0] == null || this.Parents[1] == null)
            {
                var existParents = GetUpTree(this.Parents);
                var existChilds = GetDownTree(this.Childrens);
                if (!existParents.Contains(parent) && !existChilds.Contains(parent))
                {
                    if (this.Parents[0] == null) this.Parents[0] = parent;
                    else if (this.Parents[1] == null) this.Parents[1] = parent;
                }
            }
        }

        public HashSet<Person> GetDownTree(HashSet<Person> childrens)
        {
            if (childrens.Count == 0) return new HashSet<Person>();

            HashSet<Person> listchildrens = new HashSet<Person>();
            foreach (var obj in childrens)
            {
                listchildrens.UnionWith(obj.Childrens);
            }
            if (listchildrens.Count != 0)
                listchildrens.UnionWith(GetDownTree(listchildrens));

            return listchildrens;
        }

        public void SetChildren(Person children)
        {
            if (this != children && this.Parents[0] != children && this.Parents[1] != children)
            {
                var existChilds = GetDownTree(this.Childrens);
                var existParents = GetUpTree(this.Parents);
                if (!existChilds.Contains(children) && !existParents.Contains(children))
                {
                    this.Childrens.Add(children);
                    if (this.Partner != null)
                    {
                        this.Partner.Childrens.Add(children);
                        children.SetParents(this, this.Partner);
                    }
                    else children.SetSingleParent(this);
                }
            }
        }

        public HashSet<Person> GetUpTree(Person[] parents)
        {
            if (parents.Length == 0) return new HashSet<Person>();

            HashSet<Person> listPerson = new HashSet<Person>();
            if (parents[0] != null)
            {
                listPerson.UnionWith(GetUpTree(parents[0].GetParents()));
                listPerson.Add(parents[0]);
            }

            if (parents.Length > 1 && parents[1] != null)
            {
                listPerson.UnionWith(GetUpTree(parents[1].GetParents()));
                listPerson.Add(parents[1]);
            }

            return listPerson;
        }

        public void SetPartner(Person partner)
        {
            if (this != partner && partner.Partner == null && this.Partner == null)
            {
                this.Partner = partner;
                partner.Partner = this;
            }

        }

        public string GetName()
        {
            String name = this.Name;
            return name;
        }

        public Person[] GetParents()
        {
            Person[] parents = new Person[2];
            parents[0] = this.Parents[0];
            parents[1] = this.Parents[1];
            return parents.Where(x => x != null).ToArray();
        }

        public HashSet<Person> GetChildrens()
        {
            HashSet<Person> listchildrens = this.Childrens;
            return new HashSet<Person>(listchildrens);
        }

        public HashSet<Person> GetUncles()
        {
            HashSet<Person> listUncles = new HashSet<Person>();
            foreach (Person parent in this.Parents)
            {
                if (parent == null) continue;
                foreach (Person ancestor in parent.Parents)
                {
                    if (ancestor == null) continue;
                    listUncles.UnionWith(ancestor.Childrens);
                }
            }
            listUncles.ExceptWith(this.Parents);
            HashSet<Person> listUnclesPartner = new HashSet<Person>();
            foreach (var uncle in listUncles)
            {
                if (uncle.Partner != null)
                    listUnclesPartner.Add(uncle.Partner);
            }
            listUncles.UnionWith(listUnclesPartner);
            return listUncles;
        }

        public HashSet<Person> GetCousins()
        {
            HashSet<Person> listCousins = new HashSet<Person>();
            HashSet<Person> uncles = this.GetUncles();

            foreach (Person uncle in uncles)
                foreach (Person child in uncle.Childrens)
                    if (!listCousins.Contains(child)) listCousins.Add(child);

            return listCousins;
        }

        public Person[] GetInLaws()
        {
            if (this.Partner == null)
                return new Person[2];
            return this.Partner.GetParents();
        }

        public void PrintParents()
        {
            var parents = GetParents();
            if (parents.Length == 0)
            {
                Console.WriteLine("No Parents");
                return;
            }

            foreach (var p in parents)
            {
                string status = p.Gender == Gender.Male ? "Father" : "Mother";
                Console.WriteLine(status + ": " + p.Name);
            }
        }
    }
}
