using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using QuestPDF.Companion;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ComposeAndStyleTests
    {
        // each handler is named after the element it creates, so the expected structure can be read straight from the test
        private static void SampleText(IContainer container) => container.Text("Sample");
        private static void Label(IContainer container, string label) => container.Text(label);
        private static IContainer Padded(IContainer container) => container.Padding(10);
        private static IContainer Centered(IContainer container) => container.AlignCenter();

        private class TextComponent : IComponent
        {
            public void Compose(IContainer container) => container.Text("Sample");
        }

        #region Compose

        [Test]
        public void ComposePlacesContentProducedByMethod()
        {
            var container = EmptyContainer.Create();

            container.Compose(SampleText);

            AssertStructure(container, typeof(Container), typeof(SourceCodePointer), typeof(TextBlock));
        }

        [Test]
        public void ComposeRemembersWhereItWasCalledFrom()
        {
            var container = EmptyContainer.Create();

            var lineNumber = NextLineNumber();
            container.Compose(SampleText);

            AssertSourceCodePointer(container, nameof(SampleText), lineNumber);
        }

        [Test]
        public void ComposeCalledWithLambdaIsLabelledWithNameOfInvokedMethod()
        {
            var container = EmptyContainer.Create();

            container.Compose(x => Label(x, "Sample"));

            var pointer = NestedElements(container).OfType<SourceCodePointer>().Single();
            Assert.That(pointer.DisplayMethodName, Is.EqualTo(nameof(Label)));
        }

        [Test]
        public void ComposeThrowsWhenContainerAlreadyHasContent()
        {
            var container = EmptyContainer.Create();
            container.Text("Existing");

            Assert.Throws<DocumentComposeException>(() => container.Compose(SampleText));
        }

        [Test]
        public void ComposeThrowsWhenMethodPlacesContentTwice()
        {
            var container = EmptyContainer.Create();

            Assert.Throws<DocumentComposeException>(() => container.Compose(x =>
            {
                x.Text("First");
                x.Text("Second");
            }));
        }

        [Test]
        public void ComposeThrowsWhenMethodIsMissing()
        {
            var container = EmptyContainer.Create();

            var exception = Assert.Throws<ArgumentNullException>(() => container.Compose(null));

            Assert.That(exception.ParamName, Is.EqualTo("handler"));

            // a rejected call leaves the container untouched, so it still accepts content
            container.Text("Sample");
            AssertStructure(container, typeof(TextBlock));
        }

        #endregion

        #region Component

        [Test]
        public void ComponentPlacesContentComposedByClassInstance()
        {
            var container = EmptyContainer.Create();

            container.Component(new TextComponent());

            AssertStructure(container, typeof(Container), typeof(DebugPointer), typeof(TextBlock));
        }

        [Test]
        public void ComponentPlacesContentComposedByClassType()
        {
            var container = EmptyContainer.Create();

            container.Component<TextComponent>();

            AssertStructure(container, typeof(Container), typeof(DebugPointer), typeof(TextBlock));
        }

        [Test]
        public void ComponentLabelsContentWithClassName()
        {
            var container = EmptyContainer.Create();

            container.Component(new TextComponent());

            var pointer = NestedElements(container).OfType<DebugPointer>().Single();
            Assert.That(pointer.Type, Is.EqualTo(DebugPointerType.Component));
            Assert.That(pointer.Label, Is.EqualTo(nameof(TextComponent)));
        }

        #endregion

        #region Style

        [Test]
        public void StyleWrapsFollowingContent()
        {
            var container = EmptyContainer.Create();

            container.Style(Padded).Text("Sample");

            AssertStructure(container, typeof(SourceCodePointer), typeof(Padding), typeof(TextBlock));
        }

        [Test]
        public void StyleRemembersWhereItWasCalledFrom()
        {
            var container = EmptyContainer.Create();

            var lineNumber = NextLineNumber();
            container.Style(Padded);

            AssertSourceCodePointer(container, nameof(Padded), lineNumber);
        }

        [Test]
        public void StyleNestsStylesInOrderOfCalls()
        {
            var container = EmptyContainer.Create();

            container.Style(Padded).Style(Centered).Text("Sample");

            AssertStructure(container, typeof(SourceCodePointer), typeof(Padding), typeof(SourceCodePointer), typeof(Alignment), typeof(TextBlock));
        }

        [Test]
        public void StyleThrowsWhenContainerAlreadyHasContent()
        {
            var container = EmptyContainer.Create();
            container.Text("Existing");

            Assert.Throws<DocumentComposeException>(() => container.Style(Padded));
        }

        [Test]
        public void StyleThrowsWhenMethodIsMissing()
        {
            var container = EmptyContainer.Create();

            var exception = Assert.Throws<ArgumentNullException>(() => container.Style(null));

            Assert.That(exception.ParamName, Is.EqualTo("handler"));

            // a rejected call leaves the container untouched, so it still accepts content
            container.Text("Sample");
            AssertStructure(container, typeof(TextBlock));
        }

        #endregion

        #region StyleIf

        [Test]
        public void StyleIfAppliesStyleWhenConditionIsTrue()
        {
            var container = EmptyContainer.Create();

            container.StyleIf(true, Padded).Text("Sample");

            AssertStructure(container, typeof(SourceCodePointer), typeof(Padding), typeof(TextBlock));
        }

        [Test]
        public void StyleIfSkipsStyleWhenConditionIsFalse()
        {
            var container = EmptyContainer.Create();

            container.StyleIf(false, Padded).Text("Sample");

            AssertStructure(container, typeof(TextBlock));
        }

        [Test]
        public void StyleIfDoesNotInvokeStyleMethodWhenConditionIsFalse()
        {
            var container = EmptyContainer.Create();
            var wasInvoked = false;

            container.StyleIf(false, x =>
            {
                wasInvoked = true;
                return x;
            });

            Assert.That(wasInvoked, Is.False);
        }

        [Test]
        public void StyleIfRemembersWhereItWasCalledFrom()
        {
            var container = EmptyContainer.Create();

            var lineNumber = NextLineNumber();
            container.StyleIf(true, Padded);

            AssertSourceCodePointer(container, nameof(Padded), lineNumber);
        }

        [Test]
        public void StyleIfThrowsWhenMethodIsMissingAndConditionIsTrue()
        {
            var container = EmptyContainer.Create();

            var exception = Assert.Throws<ArgumentNullException>(() => container.StyleIf(true, null));

            Assert.That(exception.ParamName, Is.EqualTo("handler"));

            // a rejected call leaves the container untouched, so it still accepts content
            container.Text("Sample");
            AssertStructure(container, typeof(TextBlock));
        }

        [Test]
        public void StyleIfAcceptsMissingMethodWhenConditionIsFalse()
        {
            // an optional style is commonly passed as: .StyleIf(style != null, style)
            Func<IContainer, IContainer> optionalStyle = null;
            var container = EmptyContainer.Create();

            container.StyleIf(optionalStyle != null, optionalStyle).Text("Sample");

            AssertStructure(container, typeof(TextBlock));
        }

        #endregion

        #region Element

        private class ForeignContainer : IContainer
        {
            public IElement? Child { get; set; }
        }

        [Test]
        public void ElementPlacesContentCreatedInAdvance()
        {
            var content = EmptyContainer.Create();
            content.Padding(10).Text("Sample");

            var container = EmptyContainer.Create();
            container.Element(content);

            AssertStructure(container, typeof(Container), typeof(Padding), typeof(TextBlock));
        }

        [Test]
        public void ElementThrowsWhenContainerAlreadyHasContent()
        {
            var content = EmptyContainer.Create();
            content.Text("Sample");

            var container = EmptyContainer.Create();
            container.Text("Existing");

            Assert.Throws<DocumentComposeException>(() => container.Element(content));
        }

        [Test]
        public void ElementThrowsWhenContentIsMissing()
        {
            var container = EmptyContainer.Create();

            Assert.Throws<ArgumentNullException>(() => container.Element((IContainer)null));
        }

        [Test]
        public void ElementThrowsWhenContentWasNotCreatedByTheLibrary()
        {
            var container = EmptyContainer.Create();

            Assert.Throws<ArgumentException>(() => container.Element(new ForeignContainer()));
        }

        [Test]
        public void ElementThrowsWhenDynamicContentIsMissing()
        {
            var container = EmptyContainer.Create();

            Assert.Throws<ArgumentNullException>(() => container.Element((IDynamicElement)null));
        }

        // The Companion app shows which line of the user's code has created each element.
        // These locations are collected only while the Companion app is attached.
        private static IDisposable AttachCompanion() => new CompanionService(port: 12500);

        [Test, NonParallelizable]
        public void ElementKeepsSourceCodeLocationOfContentCreatedInAdvance()
        {
            using var companion = AttachCompanion();

            // the variable holds the Padding element, which already knows the line that has created it
            var content = EmptyContainer.Create().Padding(10);
            content.Text("Sample");

            var locationOfContent = ((Element)content).CodeLocation;
            Assert.That(locationOfContent, Is.Not.Null);

            EmptyContainer.Create().Element(content);

            Assert.That(((Element)content).CodeLocation, Is.SameAs(locationOfContent));
        }

        [Test, NonParallelizable]
        public void ElementRemembersWhereContentWithoutSourceCodeLocationWasPlaced()
        {
            using var companion = AttachCompanion();

            // the container returned by the EmptyContainer.Create method is not created by any Fluent API call
            var content = EmptyContainer.Create();
            content.Text("Sample");

            Assert.That(((Element)content).CodeLocation, Is.Null);

            EmptyContainer.Create().Element(content);

            Assert.That(((Element)content).CodeLocation, Is.Not.Null);
        }

        #endregion

        #region Obsolete Element methods

        // The obsolete methods only forward the call.
        // The remembered location must still describe the user's code, not the forwarding method.

#pragma warning disable CS0618

        [Test]
        public void ObsoleteElementWithContentMethodBehavesLikeCompose()
        {
            var container = EmptyContainer.Create();

            var lineNumber = NextLineNumber();
            container.Element(SampleText);

            AssertStructure(container, typeof(Container), typeof(SourceCodePointer), typeof(TextBlock));
            AssertSourceCodePointer(container, nameof(SampleText), lineNumber);
        }

        [Test]
        public void ObsoleteElementWithStyleMethodBehavesLikeStyle()
        {
            var container = EmptyContainer.Create();

            var lineNumber = NextLineNumber();
            container.Element(Padded).Text("Sample");

            AssertStructure(container, typeof(SourceCodePointer), typeof(Padding), typeof(TextBlock));
            AssertSourceCodePointer(container, nameof(Padded), lineNumber);
        }

#pragma warning restore CS0618

        #endregion

        #region Assertions

        /// <summary>
        /// Returns elements placed inside the container, starting from the outermost one.
        /// </summary>
        private static IEnumerable<Element> NestedElements(IContainer container)
        {
            var element = container.Child as Element;

            while (element != null)
            {
                yield return element;
                element = (element as ContainerElement)?.Child;
            }
        }

        private static void AssertStructure(IContainer container, params Type[] expectedElementTypes)
        {
            var actualElementTypes = NestedElements(container).Select(x => x.GetType());
            Assert.That(actualElementTypes, Is.EqualTo(expectedElementTypes));
        }

        /// <summary>
        /// Call it directly above the tested invocation to get the line number of that invocation.
        /// </summary>
        private static int NextLineNumber([CallerLineNumber] int currentLineNumber = 0) => currentLineNumber + 1;

        /// <summary>
        /// Checks that the container remembers the user's code that invoked the tested method.
        /// The expected member name and file path are captured from the test that calls this method.
        /// </summary>
        private static void AssertSourceCodePointer(
            IContainer container,
            string expectedMethodName,
            int expectedLineNumber,
            [CallerMemberName] string expectedCalledFrom = "",
            [CallerFilePath] string expectedFilePath = "")
        {
            var pointer = NestedElements(container).OfType<SourceCodePointer>().Single();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(pointer.MethodName, Is.EqualTo(expectedMethodName));
                Assert.That(pointer.CalledFrom, Is.EqualTo(expectedCalledFrom));
                Assert.That(pointer.FilePath, Is.EqualTo(expectedFilePath));
                Assert.That(pointer.LineNumber, Is.EqualTo(expectedLineNumber));
            }
        }

        #endregion
    }
}
