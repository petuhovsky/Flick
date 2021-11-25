import React, {useEffect, useState} from 'react';
import {useNavigate, useSearchParams} from "react-router-dom";
import ReactTooltip from "react-tooltip";
import SearchForm from "../../components/SearchForm/SearchForm";
import ImageList from "../../components/ImageList/ImageList";
import Controller from "../../API/Controller";
import {useFetching} from "../../hooks/useFetching";
import Loader from "../../components/Loader/Loader";
import cl from "./SearchPage.module.css";

const SearchPage = (props) => {
    const [searchParams] = useSearchParams({q: ''});

    const navigate = useNavigate();

    const [_stQuery, set_stQuery] = useState(searchParams.get('q'));
    const [imageList, setImageList] = useState([]);
    const [totalHits, setTotalHits] = useState(0);

    const [fetchImages, isLoading, loadingError] = useFetching(async query => {
        console.log(`fetchImages: query=${query}`);

        const imageList = await Controller.getAllImagesFromPixabay(query);
        //const imageList = await Controller.getAllImagesFromFlickr(query);

        //await new Promise((resolve, reject) => setTimeout(resolve, 2000));
        setImageList(imageList);
    });

    useEffect(() => {
        const query = searchParams.get('q')
        set_stQuery(query);
        fetchImages(query);
    }, [searchParams]);

    const onSubmit = iQuery => {
        navigate({
            pathname: '/search',
            search: `?q=${iQuery}`
        });
    }

    const tooltipId = 'imageListTooltip';

    return (
        <div className={cl.search}>
            <div className={cl.search_header_div}>
                <h1 style={{marginRight: 20, fontWeight: 'bold'}}>Flickr search</h1>
                <SearchForm initialQuery={_stQuery} onSubmit={onSubmit}/>
            </div>

            {isLoading ?
                <div className={cl.search_loader}>
                    <Loader/>
                </div>
                :
                loadingError ?
                    <div className={cl.search_body}>
                        <h1>{loadingError}</h1>
                    </div>
                    :
                    <div className={cl.search_body}>
                        <h3 style={{marginBottom: 30}}>Showing {imageList.length} results for <i>{_stQuery}</i></h3>

                        <ImageList imageList={imageList} tooltipId={tooltipId}/>

                        <ReactTooltip id={tooltipId} type='light' delayShow={500} border={true} effect='float'/>
                    </div>
            }
        </div>
    );
};

export default SearchPage;
