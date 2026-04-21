import { Routes } from '@angular/router';
import { ProductsList } from './features/products/products-list/products-list';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Cart } from './features/cart/cart';

export const routes: Routes = [
    { path: '', component: ProductsList },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'cart', component: Cart },
    { path: 'farmer', component: FarmerDashboard }
];
