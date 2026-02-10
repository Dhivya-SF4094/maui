namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 30779, "SearchBar - CursorPosition and SelectionLength are not updated when the user types", PlatformAffected.All)]
public class Issue30779 : ContentPage
{
    Label _cursorPositionLabel;
    Label _selectionLengthLabel;
    Label _resultLabel;
    SearchBar _searchBar;

    public Issue30779()
    {
        _searchBar = new SearchBar
        {
            AutomationId = "TestSearchBar",
            Placeholder = "Type here...",
            Text = "Initial Text"
        };

        _cursorPositionLabel = new Label
        {
            Text = "CursorPosition: 0",
            AutomationId = "CursorPositionLabel"
        };

        _selectionLengthLabel = new Label
        {
            Text = "SelectionLength: 0",
            AutomationId = "SelectionLengthLabel"
        };

        _resultLabel = new Label
        {
            Text = "Ready",
            AutomationId = "ResultLabel"
        };

        // Update labels when SearchBar properties change
        _searchBar.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchBar.CursorPosition))
            {
                _cursorPositionLabel.Text = $"CursorPosition: {_searchBar.CursorPosition}";
            }
            else if (e.PropertyName == nameof(SearchBar.SelectionLength))
            {
                _selectionLengthLabel.Text = $"SelectionLength: {_searchBar.SelectionLength}";
            }
        };

        var setCursorButton = new Button
        {
            Text = "Set Cursor to Position 5",
            AutomationId = "SetCursorButton"
        };
        setCursorButton.Clicked += (s, e) =>
        {
            _searchBar.CursorPosition = 5;
            _resultLabel.Text = $"Set CursorPosition to 5, actual: {_searchBar.CursorPosition}";
        };

        var setSelectionButton = new Button
        {
            Text = "Select 3 chars at pos 2",
            AutomationId = "SetSelectionButton"
        };
        setSelectionButton.Clicked += (s, e) =>
        {
            _searchBar.CursorPosition = 2;
            _searchBar.SelectionLength = 3;
            _resultLabel.Text = $"Set selection: Cursor={_searchBar.CursorPosition}, Length={_searchBar.SelectionLength}";
        };

        var clearButton = new Button
        {
            Text = "Clear Selection",
            AutomationId = "ClearSelectionButton"
        };
        clearButton.Clicked += (s, e) =>
        {
            _searchBar.CursorPosition = _searchBar.CursorPosition;
            _searchBar.SelectionLength = 0;
            _resultLabel.Text = $"Cleared selection: Length={_searchBar.SelectionLength}";
        };

        var verifyButton = new Button
        {
            Text = "Verify Properties",
            AutomationId = "VerifyButton"
        };
        verifyButton.Clicked += (s, e) =>
        {
            _resultLabel.Text = $"Cursor={_searchBar.CursorPosition}, Selection={_searchBar.SelectionLength}, Text='{_searchBar.Text}'";
        };

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children =
            {
                new Label { Text = "SearchBar CursorPosition & SelectionLength Test", FontAttributes = FontAttributes.Bold },
                _searchBar,
                _cursorPositionLabel,
                _selectionLengthLabel,
                _resultLabel,
                setCursorButton,
                setSelectionButton,
                clearButton,
                verifyButton
            }
        };

        // Initialize labels
        _cursorPositionLabel.Text = $"CursorPosition: {_searchBar.CursorPosition}";
        _selectionLengthLabel.Text = $"SelectionLength: {_searchBar.SelectionLength}";
    }
}
