using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FishMinder.Tests.E2E;

/// <summary>
/// Base class for all FishMinder E2E tests using Playwright
/// Configured for mobile-first testing since FishMinder is primarily used on mobile devices
/// </summary>
[TestClass]
public class FishMinderTestBase : PageTest
{
    protected const string BaseUrl = "http://localhost:5001";

    /// <summary>
    /// Configure browser for mobile testing
    /// </summary>
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            // Use iPhone 12 Pro viewport for consistent mobile testing
            ViewportSize = new ViewportSize { Width = 390, Height = 844 },
            DeviceScaleFactor = 3,
            IsMobile = true,
            HasTouch = true,
            UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1"
        };
    }

    /// <summary>
    /// Initialize test - navigate to home page and wait for app to load
    /// </summary>
    [TestInitialize]
    public async Task TestInitialize()
    {
        // Navigate to the FishMinder app
        await Page.GotoAsync(BaseUrl);

        // Wait for the app to load - Blazor apps need time to initialize
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Give the Blazor app additional time to initialize
        await Page.WaitForTimeoutAsync(1000);
    }

    /// <summary>
    /// Clean up after each test - clear local storage to ensure clean state
    /// </summary>
    [TestCleanup]
    public async Task TestCleanup()
    {
        // Clear local storage to ensure clean state for next test
        await Page.EvaluateAsync("() => localStorage.clear()");
    }

    /// <summary>
    /// Helper method to perform mobile tap (more reliable than click on mobile)
    /// </summary>
    protected async Task TapAsync(string selector)
    {
        await Page.TapAsync(selector);
    }

    /// <summary>
    /// Helper method to wait for element and then tap it
    /// </summary>
    protected async Task WaitAndTapAsync(string selector, int timeoutMs = 5000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutMs });
        await Page.TapAsync(selector);
    }
}
