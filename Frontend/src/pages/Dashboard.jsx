import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import api from '../api/axios';
import ArticleCard from '../components/ArticleCard';
import Navbar from '../components/Navbar';

const Dashboard = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const category = searchParams.get('category') || 'All';

    const [articles, setArticles] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchArticles();
    }, [category]);

    const fetchArticles = async () => {
        setLoading(true);
        try {
            const endpoint = category === 'All' ? '/content/feed' : `/content/category/${category}`;
            const response = await api.get(endpoint);
            setArticles(response.data);
        } catch (error) {
            console.error('Error fetching articles', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCategoryChange = (newCategory) => {
        if (newCategory === 'All') {
            setSearchParams({});
        } else {
            setSearchParams({ category: newCategory });
        }
    };

    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
                <div className="px-4 py-6 sm:px-0">
                    <div className="flex space-x-4 mb-6 overflow-x-auto pb-2">
                        {['All', 'Technology', 'Business', 'Sports', 'General'].map((cat) => (
                            <button
                                key={cat}
                                onClick={() => handleCategoryChange(cat)}
                                className={`px-4 py-2 rounded-full text-sm font-medium ${category === cat
                                    ? 'bg-indigo-600 text-white'
                                    : 'bg-white text-gray-700 hover:bg-gray-50'
                                    }`}
                            >
                                {cat}
                            </button>
                        ))}
                    </div>

                    {loading ? (
                        <div className="flex justify-center items-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                            {articles.map((article) => (
                                <ArticleCard key={article.id} article={article} />
                            ))}
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
};

export default Dashboard;
