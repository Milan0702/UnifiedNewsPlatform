import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useState } from 'react';

const Navbar = () => {
    const { user, logout } = useAuth();
    const [searchTerm, setSearchTerm] = useState('');
    const navigate = useNavigate();

    const handleSearch = (e) => {
        if (e.key === 'Enter') {
            navigate(`/search?q=${searchTerm}`);
        }
    };

    return (
        <nav className="bg-white shadow-md">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between h-16">
                    <div className="flex items-center">
                        <Link to="/" className="flex-shrink-0 flex items-center text-xl font-bold text-indigo-600">
                            Unified News
                        </Link>
                        <div className="ml-10 flex items-center">
                            <input
                                type="text"
                                placeholder="Search news..."
                                className="w-96 px-4 py-2 rounded-lg border border-gray-300 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:bg-white transition-colors duration-200"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                onKeyDown={handleSearch}
                            />
                        </div>
                    </div>
                    <div className="flex items-center">
                        {user ? (
                            <>
                                <Link to="/feed" className="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium">My Feed</Link>
                                <Link to="/preferences" className="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium">Preferences</Link>
                                <button onClick={logout} className="ml-4 text-gray-700 hover:text-red-600 px-3 py-2 rounded-md text-sm font-medium">Logout</button>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium">Login</Link>
                                <Link to="/register" className="ml-4 bg-indigo-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-indigo-700">Register</Link>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
