﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SmartGridManager;
using WPF_Resolver.Command;
using Resolver;
using System.Threading;
using System.Net;

namespace WPF_Resolver.ViewModel
{
    class ResolverViewModel : ViewModelBase
    {
        private Thread thResolver;
        private Resolver.Resolver r;
        private string _resolverName;
        private string _resolverStatus;
        Visibility vi = new Visibility();
        private IPHostEntry _ipHost;
        private string _resolverIP;

        List<Building> peerlist = new List<Building>();

        public DelegateCommand StartResolver { get; set; }
        public DelegateCommand Exit { get; set; }

        public ResolverViewModel()
        {
            vi = Visibility.Hidden;
            _ipHost = Dns.GetHostByName(Dns.GetHostName());

            this.StartResolver = new DelegateCommand((o) => this.Start(), o => this.canStart);
            this.Exit = new DelegateCommand((o) => this.AppExit(), o => this.canExit);         
        }

        public List<Building> PeerList
        {
            get { return peerlist; }
        }

        private bool canStart
        {
            get { return true; }
        }

        public void Start()
        { 
            _resolverName = "";
            _resolverStatus = "";
            _resolverIP = "IP:  " + _ipHost.AddressList[0].ToString();

            r = new Resolver.Resolver();

            _resolverName = r.name;
            this.OnPropertyChanged("GetResolverName");

            thResolver = new Thread(r.Connect) { IsBackground = true };
            thResolver.Start();
            thResolver.Join();

            _resolverStatus = "Online...";
            vi = Visibility.Visible;
            

            this.OnPropertyChanged("GetResolverStatus");
            this.OnPropertyChanged("ImgVisibility");
            this.OnPropertyChanged("GetResolverIP");

            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
        }

        private bool canExit
        {
            get { return true; }
        }

        public void AppExit()
        {
            Application.Current.Shutdown();
        }

        public string GetResolverName
        {
            get { return _resolverName; }
            set
            {
                _resolverName = value;
                OnPropertyChanged("GetResolverName");
            }
        }

        public string GetResolverStatus
        {
            get { return _resolverStatus; }
            set
            {
                _resolverStatus = value;
                OnPropertyChanged("GetResolverStatus");
            }
        }

        public Visibility ImgVisibility
        {
            get { return vi; }
            set
            {
                vi = value;
                OnPropertyChanged("ImgVisibility");
            }
        }

        public string GetResolverIP
        {
            get { return _resolverIP; }
            set
            {
                _resolverIP = value;
                OnPropertyChanged("GetResolverIP");
            }
        }
    }
}