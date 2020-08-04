using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SampleBottomDrawer
{
    public class MainPageCode : ContentPage
    {
        BoxView Backdrop;
        Frame BottomToolbar;
        ToolbarItem OpenDrawerBtn;

        public MainPageCode()
        {
            Title = "CODE";
            On<iOS>().SetUseSafeArea(true);

            Content = new Grid
            {
                Children = {
                    new Label { Text = "Hello Code" },
                    (Backdrop = new BoxView {
                        BackgroundColor = Color.FromHex("#4B000000"),
                        Opacity = 0
                    }),
                    (BottomToolbar = new Frame {
                        HeightRequest = 200,
                        VerticalOptions = LayoutOptions.End,
                        BackgroundColor = Color.White,
                        CornerRadius = 20,
                        TranslationY = 220,
                        Padding = new Thickness(15,6),
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Padding = new Thickness(0,4),
                            Children =
                            {
                                new BoxView
                                {
                                    CornerRadius = 2,
                                    HeightRequest = 4,
                                    WidthRequest = 40,
                                    BackgroundColor = Color.LightGray,
                                    HorizontalOptions = LayoutOptions.Center
                                },
                                new Label
                                {
                                    Text = "Action",
                                    HorizontalOptions = LayoutOptions.Center,
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold
                                },
                                new TableView
                                {
                                    Intent = TableIntent.Settings,
                                    BackgroundColor = Color.White,
                                    Root = new TableRoot
                                    {
                                        new TableSection("Ring")
                                        {
                                              new TextCell { Text = "Favorite" },
                                              new TextCell { Text = "Share" }
                                        }
                                    }
                                }
                            }
                        }
                    })
                }
            };

            ToolbarItems.Add((OpenDrawerBtn = new ToolbarItem
            {
                Text = "Open Drawer"
            }));

            OpenDrawerBtn.Clicked += ToolbarItem_Clicked;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            Backdrop.GestureRecognizers.Add(tapGestureRecognizer);

            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
            BottomToolbar.GestureRecognizers.Add(panGestureRecognizer);
        }

        uint duration = 100;
        double openY = (Device.RuntimePlatform == "Android") ? 20 : 60;

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {


            if (Backdrop.Opacity == 0)
            {
                await OpenDrawer();
            }
            else
            {
                await CloseDrawer();
            }
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (isBackdropTapEnabled)
            {
                await CloseDrawer();
            }
        }

        double lastPanY = 0;
        bool isBackdropTapEnabled = true;
        async void PanGestureRecognizer_PanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                isBackdropTapEnabled = false;
                lastPanY = e.TotalY;
                Debug.WriteLine($"Running: {e.TotalY}");
                if (e.TotalY > 0)
                {
                    BottomToolbar.TranslationY = openY + e.TotalY;
                }

            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                //Debug.WriteLine($"Completed: {e.TotalY}");
                if (lastPanY < 110)
                {
                    await OpenDrawer();
                }
                else
                {
                    await CloseDrawer();
                }
                isBackdropTapEnabled = true;
            }
        }

        async Task OpenDrawer()
        {
            await Task.WhenAll
            (
                Backdrop.FadeTo(1, length: duration),
                BottomToolbar.TranslateTo(0, openY, length: duration, easing: Easing.SinIn)
            );
        }

        async Task CloseDrawer()
        {
            await Task.WhenAll
            (
                Backdrop.FadeTo(0, length: duration),
                BottomToolbar.TranslateTo(0, 260, length: duration, easing: Easing.SinIn)
            );
        }
    }
}

