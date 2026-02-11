import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';
import ArticleCard from '../components/ArticleCard';
import Navbar from '../components/Navbar';

const Feed = () => {
    const [articles, setArticles] = useState([]);
    const [loading, setLoading] = useState(true);
    const [preferences, setPreferences] = useState(null);

    useEffect(() => {
        fetchPreferencesAndArticles();
    }, []);

    const fetchPreferencesAndArticles = async () => {
        setLoading(true);
        try {
            // Fetch user preferences
            const prefsResponse = await api.get('/users/preferences');
            setPreferences(prefsResponse.data);

            // If no preferences set, don't fetch articles
            if (!prefsResponse.data ||
                (prefsResponse.data.categories.length === 0 && prefsResponse.data.sources.length === 0)) {
                setArticles([]);
                setLoading(false);
                return;
            }

            // Fetch all articles and filter client-side
            const articlesResponse = await api.get('/content/feed');
            const allArticles = articlesResponse.data;

            // Filter articles based on preferences
            const filteredArticles = allArticles.filter(article => {
                const matchesCategory = prefsResponse.data.categories.length === 0 ||
                    prefsResponse.data.categories.includes(article.category);
                const matchesSource = prefsResponse.data.sources.length === 0 ||
                    prefsResponse.data.sources.includes(article.source);

                return matchesCategory && matchesSource;
            });

            setArticles(filteredArticles);
        } catch (error) {
            console.error('Error fetching feed:', error);
        } finally {
            setLoading(false);
        }
    };

    // Show message if no preferences are set
    if (!loading && preferences &&
        preferences.categories.length === 0 && preferences.sources.length === 0) {
        return (
            <div className="min-h-screen bg-gray-100">
                <Navbar />
                <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
                    <div className="px-4 py-6 sm:px-0">
                        <div className="bg-white shadow rounded-lg p-8 text-center">
                            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
                            </svg>
                            <h2 className="mt-4 text-2xl font-bold text-gray-900">Set Your Preferences</h2>
                            <p className="mt-2 text-gray-600">
                                You haven't set any preferences yet. Customize your feed by selecting your favorite categories and sources.
                            </p>
                            <Link
                                to="/preferences"
                                className="mt-6 inline-block px-6 py-3 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
                            >
                                Go to Preferences
                            </Link>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
                <div className="px-4 py-6 sm:px-0">
                    <div className="flex justify-between items-center mb-6">
                        <h1 className="text-2xl font-bold text-gray-900">Your Personalized Feed</h1>
                        <Link
                            to="/preferences"
                            className="text-sm text-indigo-600 hover:text-indigo-700 font-medium"
                        >
                            Edit Preferences
                        </Link>
                    </div>

                    {loading ? (
                        <div className="flex justify-center items-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
                        </div>
                    ) : articles.length === 0 ? (
                        <div className="bg-white shadow rounded-lg p-8 text-center">
                            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                            </svg>
                            <h2 className="mt-4 text-xl font-semibold text-gray-900">No Articles Found</h2>
                            <p className="mt-2 text-gray-600">
                                No articles match your current preferences. Try adjusting your preferences or check back later.
                            </p>
                            <Link
                                to="/preferences"
                                className="mt-4 inline-block text-indigo-600 hover:text-indigo-700 font-medium"
                            >
                                Update Preferences
                            </Link>
                        </div>
                    ) : (
                        <>
                            <p className="text-sm text-gray-600 mb-4">
                                Showing {articles.length} article{articles.length !== 1 ? 's' : ''} based on your preferences
                            </p>
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                                {articles.map((article) => (
                                    <ArticleCard key={article.id} article={article} />
                                ))}
                            </div>
                        </>
                    )}
                </div>
            </main>
        </div>
    );
};

export default Feed;
