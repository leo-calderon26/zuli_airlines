<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';

import { useRouter } from "vue-router";

const router = useRouter();
const goBack = () => router.back();
</script>

<template>
    <PublicNavBar/>  
    <section class="bg-neutral-primary">
        <div class="py-8 px-4 mx-auto max-w-screen-2xl text-center lg:py-16">
            <h1 class="mb-6 text-4xl font-bold tracking-tighter text-heading md:text-5xl lg:text-6xl">El acceso a esta operación solo es válido para los administradores.</h1>
            <p class="mb-8 text-gold font-normal text-body md:text-xl">De click al botón de abajo para regresar a la ventana anterior.</p>
            <div class="flex flex-col space-y-4 sm:flex-row sm:justify-center sm:space-y-0 md:space-x-4">
                <button type="button" class="inline-flex items-center justify-center text-white bg-primary hover:bg-brand-strong box-border border border-transparent focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-base px-5 py-3 focus:outline-none" @click="goBack">
                    Regresar
                </button>
            </div>
        </div>
    </section>
</template>