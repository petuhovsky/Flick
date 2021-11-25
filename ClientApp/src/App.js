import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import StartSearchPage from "./pages/StartSearchPage/StartSearchPage";
import SearchPage from "./pages/SearchPage/SearchPage";

import './App.css'

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Routes>
                <Route path='/' element={<StartSearchPage/>}/>
                <Route path='/search' element={<SearchPage/>}/>
            </Routes>
        );
    }
}