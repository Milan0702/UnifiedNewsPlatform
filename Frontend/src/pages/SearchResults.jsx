import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { searchContent } from '../api/content';
import ArticleCard from '../components/ArticleCard';

const SearchResults = () => {
    const [searchParams] = useSearchParams();
    const query = searchParams.get('q');
    const [articles, setArticles] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchResults = async () => {
            if (!query) return;
            setLoading(true);
            setError(null);
            try {
                const results = await searchContent(query);
                setArticles(results);
            } catch (err) {
                setError("Failed to fetch search results.");
            } finally {
                setLoading(false);
            }
        };

        fetchResults();
    }, [query]);

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            <h1 className="text-3xl font-bold text-gray-900 mb-6">Search Results for "{query}"</h1>

            {loading && <p className="text-gray-600">Loading...</p>}

            {error && <p className="text-red-600">{error}</p>}

            {!loading && !error && articles.length === 0 && (
                <p className="text-gray-600">No results found.</p>
            )}

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {articles.map(article => (
                    <ArticleCard key={article.id} article={article} />
                ))}
            </div>
        </div>
    );
};

export default SearchResults;
