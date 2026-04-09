namespace ShoppingApp.Tests;

using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ShoppingAppTestsE2EUI
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private readonly int _delayForClass = 2000;

    private readonly string _appUrl = "http://localhost";

    [TestInitialize]
    public async Task TestInitialize()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Channel = "chrome",
            Args = new[] { "--start-maximized" }
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = null
        });

        _page = await _context.NewPageAsync();
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_page != null) await _page.CloseAsync();
        if (_context != null) await _context.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
    

    /*
    Given I am a new potential customer​
    When I sign in in the app​
    Then I should be able to log in as an application customer
    */
    [TestMethod]
    public async Task CreateNewCustomer()
    {
        // Arrange
        // Create username and password for the new customer
        var username = "customer_" + Utils.RandomString(8);
        var password = "Password123!";


        await _page!.GotoAsync(_appUrl);
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.ClickAsync("#signup");
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.WaitForSelectorAsync("#btn-signup");
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.FillAsync("#inp-username", username);
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.FillAsync("#inp-password", password);
        await _page.WaitForTimeoutAsync(_delayForClass);


        // Act
        IDialog? dialog = null;
        _page.Dialog += (_, d) => dialog = d;
        await _page.ClickAsync("#btn-signup");

        // Wait for dialog to appear
        while (dialog == null)
        {
            await _page.WaitForTimeoutAsync(100);
        }


        // Assert
        Assert.AreEqual("User registered successfully!", dialog.Message);
        await dialog.AcceptAsync();
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.ClickAsync("text=Login");
        await _page.WaitForTimeoutAsync(_delayForClass);

        // Login as newly created customer
        await _page.FillAsync("#inp-username", username);
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.FillAsync("#inp-password", password);
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.ClickAsync("#btn-login");
        await _page.WaitForTimeoutAsync(_delayForClass);

        await _page.WaitForSelectorAsync($"h2:has-text('Welcome, {username}!')");
        await _page.WaitForTimeoutAsync(_delayForClass);
    }
}
