namespace QuestPDF.LayoutTests;

[TestFixture]
public class RowTests
{
    #region General Item Positioning
    
    [Test]
    public void SingleConstantItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(40).Mock("a").Height(30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(40, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 30);
                    });
            });
    }
    
    [Test]
    public void MultipleConstantItems()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(20).Mock("a").Height(30);
                    row.ConstantItem(30).Mock("b").Height(40);
                    row.ConstantItem(40).Mock("c").Height(20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(90, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(20, 40);
                        page.Mock("b").Position(20, 0).Size(30, 40);
                        page.Mock("c").Position(50, 0).Size(40, 40);
                    });
            });
    }
    
    [Test]
    public void SingleRelativeItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem().Mock("a").Height(30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 30);
                    });
            });
    }
    
    [Test]
    public void TwoRelativeItems()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem(2).Mock("a").Height(40);
                    row.RelativeItem(3).Mock("b").Height(50);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 50);
                        page.Mock("b").Position(40, 0).Size(60, 50);
                    });
            });
    }
    
    [Test]
    public void SingleAutoItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.AutoItem().Mock("a").Size(60, 40);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(60, 40);
                    });
            });
    }
    
    [Test]
    public void RelativeItemFillsRemainingSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(20).Mock("a").Height(30);
                    row.ConstantItem(30).Mock("b").Height(50);
                    row.RelativeItem().Mock("c").Height(40);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(20, 50);
                        page.Mock("b").Position(20, 0).Size(30, 50);
                        page.Mock("c").Position(50, 0).Size(50, 50);
                    });
            });
    }
    
    [Test]
    public void RelativeItemsSplitRemainingSpaceAccordingToProportions()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(30).Mock("a").Height(60);
                    row.RelativeItem(4).Mock("b").Height(40);
                    row.RelativeItem(3).Mock("c").Height(30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(30, 60);
                        page.Mock("b").Position(30, 0).Size(40, 60);
                        page.Mock("c").Position(70, 0).Size(30, 60);
                    });
            });
    }
    
    [Test]
    public void ComplexItems()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(30).Mock("a").Height(60);
                    row.RelativeItem(1).Mock("b").Height(40);
                    row.RelativeItem(2).Mock("c").Height(30);
                    row.ConstantItem(20).Mock("d").Height(30);
                    row.RelativeItem(3).Mock("e").Height(20);
                    row.RelativeItem(2).Mock("f").Height(70);
                    row.AutoItem().Mock("g").Size(40, 50);
                    row.AutoItem().Mock("h").Size(30, 40);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 70)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(30, 70);
                        page.Mock("b").Position(30, 0).Size(10, 70);
                        page.Mock("c").Position(40, 0).Size(20, 70);
                        page.Mock("d").Position(60, 0).Size(20, 70);
                        page.Mock("e").Position(80, 0).Size(30, 70);
                        page.Mock("f").Position(110, 0).Size(20, 70);
                        page.Mock("g").Position(130, 0).Size(40, 70);
                        page.Mock("h").Position(170, 0).Size(30, 70);
                    });
            });
    }
    
    #endregion
    
    #region Drawing On Multiple Pages
    
    [Test]
    public void OneItemSpansTwoPages()
    {
        LayoutTest
            .HavingSpaceOfSize(80, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem().Mock("a").SolidBlock(height: 40);
                    row.RelativeItem().Mock("b").SolidBlock(height: 80);
                    row.RelativeItem(2).Mock("c").ContinuousBlock(20, 140);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(20, 100);
                        page.Mock("b").Position(20, 0).Size(20, 100);
                        page.Mock("c").Position(40, 0).Size(40, 100);
                    });
            
                document
                    .Page()
                    .RequiredAreaSize(80, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(20, 40);
                        page.Mock("b").Position(20, 0).Size(20, 40);
                        page.Mock("c").Position(40, 0).Size(40, 40);
                    });
            });  
    }
    
    #endregion

    #region Spacing

    [Test]
    public void NoSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(10).Mock("a").SolidBlock(height: 40);
                    row.ConstantItem(20).Mock("b").SolidBlock(height: 50);
                    row.ConstantItem(30).Mock("c").SolidBlock(height: 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(10, 50);
                        page.Mock("b").Position(10, 0).Size(20, 50);
                        page.Mock("c").Position(30, 0).Size(30, 50);
                    });
            });
    }
    
    [Test]
    public void NormalSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.Spacing(10);
                    
                    row.ConstantItem(10).Mock("a").SolidBlock(height: 40);
                    row.ConstantItem(20).Mock("b").SolidBlock(height: 50);
                    row.ConstantItem(30).Mock("c").SolidBlock(height: 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(80, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(10, 50);
                        page.Mock("b").Position(20, 0).Size(20, 50);
                        page.Mock("c").Position(50, 0).Size(30, 50);
                    });
            });
    }
    
    [Test]
    public void BiggerSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.Spacing(15);
                    
                    row.ConstantItem(10).Mock("a").SolidBlock(height: 40);
                    row.ConstantItem(20).Mock("b").SolidBlock(height: 50);
                    row.ConstantItem(30).Mock("c").SolidBlock(height: 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(90, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(10, 50);
                        page.Mock("b").Position(25, 0).Size(20, 50);
                        page.Mock("c").Position(60, 0).Size(30, 50);
                    });
            });
    }
    
    [Test]
    public void SpacingDoesNotFitInAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.Spacing(20);

                    row.ConstantItem(30).Height(50);
                    row.ConstantItem(30).Height(50);
                    row.ConstantItem(30).Height(50);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void SpacingIsLargerThanAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.Spacing(200);
                    
                    row.ConstantItem(10).Mock("a").SolidBlock(height: 40);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void NotEnoughSpaceForRelativeItemsAfterApplyingSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.Spacing(20);
                    
                    row.ConstantItem(40);
                    row.ConstantItem(40);
                    row.RelativeItem();
                });
            })
            .ExpectLayoutException("One of the items has a negative size, indicating insufficient horizontal space. Usually, constant items require more space than is available, potentially causing other content to overflow.");
    }

    #endregion
    
    #region Paging
    
    [Test]
    public void OneItemRequiresTwoPages()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem(2).Mock("a").Height(30);
                    row.RelativeItem(3).Mock("b").ContinuousBlock(height: 140);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 100);
                        page.Mock("b").Position(40, 0).Size(60, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 40);
                        page.Mock("b").Position(40, 0).Size(60, 40);
                    });
            });
    } 
    
    [Test]
    public void ItemsRequireMultiplePages()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem(2).Mock("a").ContinuousBlock(height: 230);
                    row.RelativeItem(3).Mock("b").ContinuousBlock(height: 140);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 100);
                        page.Mock("b").Position(40, 0).Size(60, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 100);
                        page.Mock("b").Position(40, 0).Size(60, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(40, 30);
                        page.Mock("b").Position(40, 0).Size(60, 30);
                    });
            });
    }
    
    [Test]
    public void ItemHeightExceedsAvailableHeight()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(40).Height(50);
                    row.ConstantItem(40).Height(200); // <-
                });
            })
            .ExpectLayoutException("The available vertical space is less than the minimum height.");
    }
    
    #endregion
    
    #region Right-To-Left Direction
    
    [Test]
    public void RightToLeftDirection()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.ContentFromRightToLeft().Shrink().Row(row =>
                {
                    row.ConstantItem(20).Mock("a").Height(30);
                    row.ConstantItem(30).Mock("b").Height(40);
                    row.ConstantItem(40).Mock("c").Height(20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(90, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(80, 0).Size(20, 40);
                        page.Mock("b").Position(50, 0).Size(30, 40);
                        page.Mock("c").Position(10, 0).Size(40, 40);
                    });
            });
    } 
    
    #endregion
    
    #region Layout Exceptions
    
    [Test]
    public void ConstantItemOfTooLargeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(200);  // <-
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void ConstantItemHasChildOfTooLargeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(30).Width(20);
                    row.ConstantItem(40).Width(200); // <-
                });
            })
            .ExpectLayoutException("The available horizontal space is less than the minimum width.");
    }
    
    [Test]
    public void SumOfConstantItemsExceedsAvailableWidth()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(40);
                    row.ConstantItem(40);
                    row.ConstantItem(40);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void RelativeItemHasChildOfTooLargeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem(2).Width(30);
                    row.RelativeItem(3).Width(200); // <-
                });
            })
            .ExpectLayoutException("The available horizontal space is less than the minimum width.");
    }
    
    [Test]
    public void AutoItemHasChildOfTooLargeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(30);
                    row.AutoItem().Width(80);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void SumOfAutoItemsExceedsAvailableWidth()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.AutoItem().Width(30);
                    row.AutoItem().Width(40);
                    row.AutoItem().Width(50);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    [Test]
    public void SumOfVariousItemsExceedsAvailableWidth()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(60);
                    row.AutoItem().Width(50);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }
    
    #endregion
    
    #region Corner Cases
    
    [Test]
    public void NoItems()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    // <-
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 0)
                    .Content(page => { });
            });
    }
    
    [Test]
    public void RerenderingOfFullyDrawnRow()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.RelativeItem().Mock("a").Row(innerRow =>
                    {
                        innerRow.RelativeItem().Mock("b").SolidBlock(20, 50);
                        innerRow.RelativeItem().Mock("c").SolidBlock(15, 40);
                    });
                    
                    row.RelativeItem().Mock("d").ContinuousBlock(40, 140);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 100);
                        page.Mock("b").Position(0, 0).Size(25, 100);
                        page.Mock("c").Position(25, 0).Size(25, 100);
                        page.Mock("d").Position(50, 0).Size(50, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 40);
                        page.Mock("d").Position(50, 0).Size(50, 40);
                    });
            });
    }
    
    #endregion
}