from django.conf import settings
from django.core.paginator import EmptyPage, PageNotAnInteger, Paginator
from django.shortcuts import render
from wagtail.contrib.search_promotions.models import Query
from wagtail.models import Page

from bcwagtail.releasenotes.models import releasenotesPage
from bcwagtail.newfeatures.models import NewFeaturePage
from bcwagtail.frameworks.models import FrameworkPage


def search(request):
    # Search
    search_query = request.GET.get("q", None)
    if search_query:
        if "elasticsearch" in settings.WAGTAILSEARCH_BACKENDS["default"]["BACKEND"]:
            # In production, use ElasticSearch and a simplified search query, per
            # https://docs.wagtail.org/en/stable/topics/search/backends.html
            # like this:
            search_results = Page.objects.live().search(search_query)
        else:
            # If we aren't using ElasticSearch for the demo, fall back to native db search.
            # But native DB search can't search specific fields in our models on a `Page` query.
            # So for demo purposes ONLY, we hard-code in the model names we want to search.
            releasenotes_results = releasenotesPage.objects.live().search(search_query)
            releasenotes_page_ids = [p.page_ptr.id for p in releasenotes_results]

            newfeature_results = NewFeaturePage.objects.live().search(search_query)
            newfeature_page_ids = [p.page_ptr.id for p in newfeature_results]

            framework_results = FrameworkPage.objects.live().search(search_query)
            framework_result_ids = [p.page_ptr.id for p in framework_results]

            page_ids = releasenotes_page_ids + newfeature_page_ids + framework_result_ids
            search_results = Page.objects.live().filter(id__in=page_ids)

        query = Query.get(search_query)

        # Record hit
        query.add_hit()

    else:
        search_results = Page.objects.none()

    # Pagination
    page = request.GET.get("page", 1)
    paginator = Paginator(search_results, 10)
    try:
        search_results = paginator.page(page)
    except PageNotAnInteger:
        search_results = paginator.page(1)
    except EmptyPage:
        search_results = paginator.page(paginator.num_pages)

    return render(
        request,
        "search/search_results.html",
        {
            "search_query": search_query,
            "search_results": search_results,
        },
    )
