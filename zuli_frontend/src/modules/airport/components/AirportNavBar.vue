<template>
<div class="sm:hidden">
    <label for="tabs-icons" class="sr-only">Seleccionar opción</label>
    <select id="tabs-icons" class="block w-full px-3 py-2.5 bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand shadow-xs placeholder:text-body">
        <option>Listar Aeropuertos</option>
        <option>Crear Aeropuerto</option>
    </select>
</div>
<ul class="hidden text-sm font-medium text-center text-body sm:flex -space-x-px">
    <li class="w-full focus-within:z-10">
        <router-link to="/admin/airports">
            <a href="#" class="inline-flex items-center justify-center w-full text-body bg-neutral-primary-soft border border-default rounded-s-base hover:bg-purple hover:text-heading focus:ring-4 focus:ring-neutral-secondary-strong font-medium leading-5 text-sm px-4 py-2.5 focus:outline-none">
                <p class="text-primary">Listar Aeropuertos</p>
            </a>
        </router-link>
    </li>
    <li class="w-full focus-within:z-10">
        <router-link to="/admin/create-airport">
            <a href="#" class="inline-flex items-center justify-center w-full text-body bg-neutral-primary-soft border border-default hover:bg-purple hover:text-heading focus:ring-4 focus:ring-neutral-secondary-strong font-medium leading-5 text-sm px-4 py-2.5 focus:outline-none">
                <p class="text-primary">Crear Aeropuerto</p>
            </a>
        </router-link>
    </li>
</ul>
</template>