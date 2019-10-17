namespace BetterBlueprints.Override
{
    class SelectPageNumberEx : SelectPageNumber
    {
        /*protected override void Update()
        {
            SelfOvered = IsOverCollider();
            PageIsActive = (MyPageNew && MyPageNew.activeSelf);
            if ((TheForest.Utils.Input.GetButtonDown("Fire1") || (TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetButtonDown("Take"))) && SelfOvered)
            {
                SelfOvered = false;
                PageIsActive = false;
                OnClick();
                UpdateActivePage();
            }
            if (Highlighted && ((HighlightedPage && HighlightedPage.activeSelf) || (!HighlightedPage && PageIsActive)))
            {
                Unhighlight();
            }
            RefreshVisuals();
        }

        private void UpdateActivePage()
        {
            for (int page = 0; page <= Pages.transform.childCount; page++)
            {
                if (Pages.transform.GetChild(page).gameObject.activeSelf)
                {
                    BetterBlueprintsCore.CurrentPage = page;
                    ModAPI.Console.Write("Active: " + BetterBlueprintsCore.CurrentPage);
                    return;
                }
            }
        }*/
    }
}
