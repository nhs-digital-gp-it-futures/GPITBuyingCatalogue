from __future__ import unicode_literals

from django.contrib import messages
from django.db import models
from django.shortcuts import redirect, render
from modelcluster.contrib.taggit import ClusterTaggableManager
from modelcluster.fields import ParentalKey
from taggit.models import Tag, TaggedItemBase
from wagtail.admin.panels import FieldPanel, MultipleChooserPanel
from wagtail.contrib.routable_page.models import RoutablePageMixin, route
from wagtail.fields import StreamField
from wagtail.models import Orderable, Page
from wagtail.search import index

from bcwagtail.base.blocks import BaseStreamBlock


class releasenotesPersonRelationship(Orderable, models.Model):
    """
    This defines the relationship between the `Person` within the `base`
    app and the releasenotesPage below. This allows people to be added to a releasenotesPage.

    We have created a two way relationship between releasenotesPage and Person using
    the ParentalKey and ForeignKey
    """

    page = ParentalKey(
        "releasenotesPage", related_name="releasenotes_person_relationship", on_delete=models.CASCADE
    )
    person = models.ForeignKey(
        "base.Person", related_name="person_releasenotes_relationship", on_delete=models.CASCADE
    )
    panels = [FieldPanel("person")]


class releasenotesPageTag(TaggedItemBase):
    """
    This model allows us to create a many-to-many relationship between
    the releasenotesPage object and tags. There's a longer guide on using it at
    https://docs.wagtail.org/en/stable/reference/pages/model_recipes.html#tagging
    """

    content_object = ParentalKey(
        "releasenotesPage", related_name="tagged_items", on_delete=models.CASCADE
    )


class releasenotesPage(Page):
    """
    A releasenotes Page

    We access the Person object with an inline panel that references the
    ParentalKey's related_name in releasenotesPersonRelationship. More docs:
    https://docs.wagtail.org/en/stable/topics/pages.html#inline-models
    """

    introduction = models.TextField(help_text="Text to describe the page", blank=True)
    image = models.ForeignKey(
        "wagtailimages.Image",
        null=True,
        blank=True,
        on_delete=models.SET_NULL,
        related_name="+",
        help_text="Landscape mode only; horizontal width between 1000px and 3000px.",
    )
    body = StreamField(
        BaseStreamBlock(), verbose_name="Page body", blank=True, use_json_field=True
    )
    subtitle = models.CharField(blank=True, max_length=255)
    tags = ClusterTaggableManager(through=releasenotesPageTag, blank=True)
    date_published = models.DateField("Date article published", blank=True, null=True)

    content_panels = Page.content_panels + [
        FieldPanel("subtitle"),
        FieldPanel("introduction"),
        FieldPanel("image"),
        FieldPanel("body"),
        FieldPanel("date_published"),
        MultipleChooserPanel(
            "releasenotes_person_relationship",
            chooser_field_name="person",
            heading="Authors",
            label="Author",
            panels=None,
            min_num=1,
        ),
        FieldPanel("tags"),
    ]

    search_fields = Page.search_fields + [
        index.SearchField("body"),
    ]

    def authors(self):
        """
        Returns the releasenotesPage's related people. Again note that we are using
        the ParentalKey's related_name from the releasenotesPersonRelationship model
        to access these objects. This allows us to access the Person objects
        with a loop on the template. If we tried to access the releasenotes_person_
        relationship directly we'd print `releasenotes.releasenotesPersonRelationship.None`
        """
        # Only return authors that are not in draft
        return [
            n.person
            for n in self.releasenotes_person_relationship.filter(
                person__live=True
            ).select_related("person")
        ]

    @property
    def get_tags(self):
        """
        Similar to the authors function above we're returning all the tags that
        are related to the releasenotes post into a list we can access on the template.
        We're additionally adding a URL to access releasenotesPage objects with that tag
        """
        tags = self.tags.all()
        base_url = self.get_parent().url
        for tag in tags:
            tag.url = f"{base_url}tags/{tag.slug}/"
        return tags

    # Specifies parent to releasenotesPage as being releasenotesIndexPages
    parent_page_types = ["releasenotesIndexPage"]

    # Specifies what content types can exist as children of releasenotesPage.
    # Empty list means that no child content types are allowed.
    subpage_types = []


class releasenotesIndexPage(RoutablePageMixin, Page):
    """
    Index page for releasenotess.
    We need to alter the page model's context to return the child page objects,
    the releasenotesPage objects, so that it works as an index page

    RoutablePageMixin is used to allow for a custom sub-URL for the tag views
    defined above.
    """

    introduction = models.TextField(help_text="Text to describe the page", blank=True)
    image = models.ForeignKey(
        "wagtailimages.Image",
        null=True,
        blank=True,
        on_delete=models.SET_NULL,
        related_name="+",
        help_text="Landscape mode only; horizontal width between 1000px and 3000px.",
    )

    content_panels = Page.content_panels + [
        FieldPanel("introduction"),
        FieldPanel("image"),
    ]

    # Specifies that only releasenotesPage objects can live under this index page
    subpage_types = ["releasenotesPage"]

    # Defines a method to access the children of the page (e.g. releasenotesPage
    # objects). On the demo site we use this on the HomePage
    def children(self):
        return self.get_children().specific().live()

    # Overrides the context to list all child items, that are live, by the
    # date that they were published
    # https://docs.wagtail.org/en/stable/getting_started/tutorial.html#overriding-context
    def get_context(self, request):
        context = super(releasenotesIndexPage, self).get_context(request)
        context["posts"] = (
            releasenotesPage.objects.descendant_of(self).live().order_by("-date_published")
        )
        return context

    # This defines a Custom view that utilizes Tags. This view will return all
    # related releasenotesPages for a given Tag or redirect back to the releasenotesIndexPage.
    # More information on RoutablePages is at
    # https://docs.wagtail.org/en/stable/reference/contrib/routablepage.html
    @route(r"^tags/$", name="tag_archive")
    @route(r"^tags/([\w-]+)/$", name="tag_archive")
    def tag_archive(self, request, tag=None):

        try:
            tag = Tag.objects.get(slug=tag)
        except Tag.DoesNotExist:
            if tag:
                msg = 'There are no releasenotes posts tagged with "{}"'.format(tag)
                messages.add_message(request, messages.INFO, msg)
            return redirect(self.url)

        posts = self.get_posts(tag=tag)
        context = {"self": self, "tag": tag, "posts": posts}
        return render(request, "releasenotes/releasenotes_index_page.html", context)

    def serve_preview(self, request, mode_name):
        # Needed for previews to work
        return self.serve(request)

    # Returns the child releasenotesPage objects for this releasenotesPageIndex.
    # If a tag is used then it will filter the posts by tag.
    def get_posts(self, tag=None):
        posts = releasenotesPage.objects.live().descendant_of(self)
        if tag:
            posts = posts.filter(tags=tag)
        return posts

    # Returns the list of Tags for all child posts of this releasenotesPage.
    def get_child_tags(self):
        tags = []
        for post in self.get_posts():
            # Not tags.append() because we don't want a list of lists
            tags += post.get_tags
        tags = sorted(set(tags))
        return tags
