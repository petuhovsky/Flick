import React, {useState} from 'react';
import {useNavigate} from "react-router-dom";
import SearchForm from "../../components/SearchForm/SearchForm";
import cl from './StartSearchPage.module.css'

const StartSearchPage = () => {

    const [query] = useState('');
    const navigate = useNavigate();

    const onSubmit = query => {
        navigate({
            pathname: '/search',
            search: `?q=${query}`,
        });
    }

    return (
        <div className={cl.search}>
            <div className={cl.search_div}>
                <h1><b>Flickr search</b></h1>
                <SearchForm initialQuery={query} onSubmit={onSubmit}/>
            </div>
        </div>
    );
};

export default StartSearchPage;