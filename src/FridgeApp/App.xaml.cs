using Autofac;
using Autofac.Core;
using FridgeApp.Services;
using FridgeApp.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FridgeApp
{
	public partial class App : Application
	{
		// IContainer and ContainerBuilder are provided by Autofac
		static IContainer container;
		static readonly ContainerBuilder builder;

		//Custom event that is raised when the application is starting
		private event EventHandler Starting = delegate { };

		static App()
		{
			var fridgeDal = new MockFridgeDAL(true);
			//fridgeDal.CreateUser(MockFridgeDAL.GetDefaultUser());

			builder = new ContainerBuilder();
			builder.RegisterInstance<IFridgeDAL>(fridgeDal);
			RegisterType<IMainViewModel, MainViewModel>();
			RegisterType<IItemsViewModel, ItemsViewModel>();
			RegisterType<ISettingsViewModel, SettingsViewModel>();
			RegisterType<IFridgeViewModel, FridgeViewModel>();
			RegisterType<IItemViewModel, ItemViewModel>();
			RegisterType<IUserViewModel, UserViewModel>();
		}

		public App()
		{
			InitializeComponent();

			DependencyResolver.ResolveUsing(type =>
			{
				object res = container.IsRegistered(type) ? container.Resolve(type) : null;
				return res;
			});

			MainPage = new AppShell();

		}

		protected override void OnStart()
		{
			//subscribe to event
			Starting += OnStarting;
			//raise event
			Starting(this, EventArgs.Empty);
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}

		public static void RegisterType<T>() where T : class
		{
			builder.RegisterType<T>();
		}

		public static void RegisterType<TInterface, T>() where TInterface : class where T : class, TInterface
		{
			builder.RegisterType<T>().As<TInterface>();
		}

		public static void RegisterTypeWithParameters<T>(Type param1Type, object param1Value, Type param2Type, string param2Name) where T : class
		{
			builder.RegisterType<T>()
						 .WithParameters(new List<Parameter>()
			{
						new TypedParameter(param1Type, param1Value),
						new ResolvedParameter(
								(pi, ctx) => pi.ParameterType == param2Type && pi.Name == param2Name,
								(pi, ctx) => ctx.Resolve(param2Type))
			});
		}

		public static void RegisterTypeWithParameters<TInterface, T>(Type param1Type, object param1Value, Type param2Type, string param2Name) where TInterface : class where T : class, TInterface
		{
			builder.RegisterType<T>()
						 .WithParameters(new List<Parameter>()
			{
						new TypedParameter(param1Type, param1Value),
						new ResolvedParameter(
								(pi, ctx) => pi.ParameterType == param2Type && pi.Name == param2Name,
								(pi, ctx) => ctx.Resolve(param2Type))
			}).As<TInterface>();
		}

		public static void BuildContainer()
		{
			container = builder.Build();
		}

		private async void OnStarting(object sender, EventArgs args)
		{
			//unsubscribe from event
			Starting -= OnStarting;

			var fridgeDal = container.Resolve<IFridgeDAL>();
			var user = await fridgeDal.GetUserAsync(); 

			if(user == null)
			{
				((AppShell)Shell.Current).OpenUserPage();
			}
		}
	}
}
