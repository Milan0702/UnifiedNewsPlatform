import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';
import Navbar from '../components/Navbar';

const Preferences = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [message, setMessage] = useState('');

    const [selectedCategories, setSelectedCategories] = useState([]);
    const [selectedSources, setSelectedSources] = useState([]);

    const availableCategories = ['Technology', 'Business', 'Sports', 'General'];
    const availableSources = [
        'BBC News', 'CNN', 'TechCrunch', 'The Verge', 'Wired',
        'Bloomberg', 'Reuters', 'ESPN', 'NPR', 'The Guardian'
    ];

    useEffect(() => {
        fetchPreferences();
    }, []);

    const fetchPreferences = async () => {
        try {
            const response = await api.get('/users/preferences');
            if (response.data) {
                setSelectedCategories(response.data.categories || []);
                setSelectedSources(response.data.sources || []);
            }
        } catch (error) {
            console.error('Error fetching preferences:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCategoryToggle = (category) => {
        setSelectedCategories(prev =>
            prev.includes(category)
                ? prev.filter(c => c !== category)
                : [...prev, category]
        );
    };

    const handleSourceToggle = (source) => {
        setSelectedSources(prev =>
            prev.includes(source)
                ? prev.filter(s => s !== source)
                : [...prev, source]
        );
    };

    const handleSave = async () => {
        setSaving(true);
        setMessage('');
        try {
            await api.put('/users/preferences', {
                categories: selectedCategories,
                sources: selectedSources
            });
            setMessage('Preferences saved successfully!');
            setTimeout(() => {
                navigate('/feed');
            }, 1500);
        } catch (error) {
            console.error('Error saving preferences:', error);
            setMessage('Failed to save preferences. Please try again.');
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-100">
                <Navbar />
                <div className="flex justify-center items-center h-64">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
                <div className="px-4 py-6 sm:px-0">
                    <div className="bg-white shadow rounded-lg p-6">
                        <h1 className="text-2xl font-bold text-gray-900 mb-6">Your Preferences</h1>

                        {message && (
                            <div className={`mb-4 p-4 rounded-md ${message.includes('success') ? 'bg-green-50 text-green-800' : 'bg-red-50 text-red-800'}`}>
                                {message}
                            </div>
                        )}

                        {/* Categories Section */}
                        <div className="mb-8">
                            <h2 className="text-lg font-semibold text-gray-900 mb-4">Favorite Categories</h2>
                            <p className="text-sm text-gray-600 mb-4">Select the news categories you're interested in</p>
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                                {availableCategories.map((category) => (
                                    <button
                                        key={category}
                                        onClick={() => handleCategoryToggle(category)}
                                        className={`p-3 rounded-lg border-2 transition-all ${selectedCategories.includes(category)
                                                ? 'border-indigo-600 bg-indigo-50 text-indigo-700'
                                                : 'border-gray-300 bg-white text-gray-700 hover:border-indigo-300'
                                            }`}
                                    >
                                        <div className="flex items-center justify-center">
                                            <span className="font-medium">{category}</span>
                                            {selectedCategories.includes(category) && (
                                                <svg className="ml-2 h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                                                    <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                                </svg>
                                            )}
                                        </div>
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Sources Section */}
                        <div className="mb-8">
                            <h2 className="text-lg font-semibold text-gray-900 mb-4">Favorite Sources</h2>
                            <p className="text-sm text-gray-600 mb-4">Select your preferred news sources</p>
                            <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                                {availableSources.map((source) => (
                                    <button
                                        key={source}
                                        onClick={() => handleSourceToggle(source)}
                                        className={`p-3 rounded-lg border-2 transition-all ${selectedSources.includes(source)
                                                ? 'border-indigo-600 bg-indigo-50 text-indigo-700'
                                                : 'border-gray-300 bg-white text-gray-700 hover:border-indigo-300'
                                            }`}
                                    >
                                        <div className="flex items-center justify-center">
                                            <span className="text-sm font-medium">{source}</span>
                                            {selectedSources.includes(source) && (
                                                <svg className="ml-2 h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                                                    <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                                </svg>
                                            )}
                                        </div>
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex justify-end space-x-4">
                            <button
                                onClick={() => navigate('/')}
                                className="px-6 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSave}
                                disabled={saving || (selectedCategories.length === 0 && selectedSources.length === 0)}
                                className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
                            >
                                {saving ? 'Saving...' : 'Save Preferences'}
                            </button>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default Preferences;
