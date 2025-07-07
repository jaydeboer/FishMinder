using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FishMinder.Tests.E2E;

/// <summary>
/// Tests for the Home page functionality - Trip Management
/// Testing the most basic features: Start Trip and End Trip
/// </summary>
[TestClass]
public class HomePageTests : FishMinderTestBase
{
    [TestMethod]
    public async Task Test01_HomePage_LoadsSuccessfully()
    {
        // Verify the home page loads with expected elements - use specific selector for dashboard title
        await Expect(Page.Locator(".dashboard-title")).ToContainTextAsync("Welcome to FishMinder");

        // Verify dashboard elements are present
        await Expect(Page.Locator(".dashboard")).ToBeVisibleAsync();
        await Expect(Page.Locator(".dashboard-title")).ToBeVisibleAsync();
        await Expect(Page.Locator(".dashboard-subtitle")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Test02_StartTrip_CreatesNewTrip()
    {
        // Verify initial state - no active trip
        await Expect(Page.Locator("text=No active trip")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Start Trip")).ToBeVisibleAsync();
        
        // Start a new trip by tapping the Start Trip button
        await TapAsync("text=Start Trip");
        
        // Wait for the trip to be created and UI to update
        await Page.WaitForTimeoutAsync(2000);
        
        // Verify trip was started
        await Expect(Page.Locator("text=End Trip")).ToBeVisibleAsync();
        await Expect(Page.Locator(".trip-status")).ToContainTextAsync("Fishing Trip");

        // Verify trip details are shown
        await Expect(Page.Locator("text=Started")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=0 anglers, 0 catches")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Test03_EndTrip_EndsActiveTrip()
    {
        // First start a trip
        await TapAsync("text=Start Trip");
        await Page.WaitForTimeoutAsync(2000);
        
        // Verify trip is active
        await Expect(Page.Locator("text=End Trip")).ToBeVisibleAsync();
        
        // End the trip - this will show a confirmation dialog
        await TapAsync("text=End Trip");
        
        // Handle the confirmation dialog
        Page.Dialog += async (_, dialog) =>
        {
            await dialog.AcceptAsync();
        };
        
        // Wait for the trip to end and UI to update
        await Page.WaitForTimeoutAsync(2000);
        
        // Verify trip was ended
        await Expect(Page.Locator("text=No active trip")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Start Trip")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Test04_QuickStats_DisplayCorrectly()
    {
        // Verify quick stats are displayed
        await Expect(Page.Locator("text=Quick Stats")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Total Catches")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Species")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Anglers")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Trips")).ToBeVisibleAsync();
        
        // Verify initial stats show zeros
        var statNumbers = Page.Locator(".stat-number");
        await Expect(statNumbers.First).ToContainTextAsync("0");
    }

    [TestMethod]
    public async Task Test05_QuickActions_ArePresent()
    {
        // Verify quick action buttons are present
        await Expect(Page.Locator("text=Quick Actions")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Add Catch")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Manage Anglers")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=View Species")).ToBeVisibleAsync();
    }
}
