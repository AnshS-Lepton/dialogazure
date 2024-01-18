
const modal = document.querySelector('.deleteModal');
const closeModal = document.querySelector('.close-modal');
const overlay = document.querySelector('.overlay');
const showModal = document.querySelectorAll('.show-modal');
const cancel = document.querySelector('.cancel');

// open modal code 
const openModal = function () {
    modal.classList.remove('hidden');
    overlay.classList.remove('hidden');
    modal.classList.add('visible','trns');
   // modal.classList.add('trns');
    overlay.classList.add('visible');
}

// close modal code 
const closeAllModal = function () {
    modal.classList.add('hidden');
    overlay.classList.add('hidden');
    modal.classList.remove('visible', 'trns');
    cancel.classList.remove('visible', 'trns');
   // modal.classList.remove('trns');
    overlay.classList.remove('visible');

    //set the textbpx value to empty and disable buttons and add class
    if ($(dynamicForm.DE.btnDeleteControl).hasClass('btn--box_ok')) {
        document.getElementById("btnDeleteControl").classList.remove('btn--box_ok');
        document.getElementById("btnDeleteControl").classList.add('btn--box_ok_disable');
    }
    $(dynamicForm.DE.btnDeleteControl).attr('disabled', true);
    $(dynamicForm.DE.deleteControlName).val('');
}

//for (let i = 0; i < showModal.length; i++)
//    showModal[i].addEventListener('click', openModal);

closeModal.addEventListener('click', closeAllModal);
overlay.addEventListener('click', closeAllModal);
cancel.addEventListener('click', closeAllModal);


/*const modal = document.querySelector('.modal');
const btnCloseModal = document.querySelector('.close-modal');
const overlay = document.querySelector('.overlay');
const btnShowModal = document.querySelectorAll('.show-modal');

const openModal = function () {
    modal.classList.remove('hidden');
    overlay.classList.remove('hidden');
}

const closeModal = function () {
    modal.classList.add('hidden');
    overlay.classList.add('hidden');
}

for (let i = 0; i < btnShowModal.length; i++)
    btnShowModal[i].addEventListener('click', openModal);

btnCloseModal.addEventListener('click', closeModal)
*/


// Key press function - using javascript

/*document.addEventListener('keydown', function (e) {
    console.log(e.key)

    if (e.key === 'Escape')
        if (!modal.classList.contains('hidden')) {
            closeAllModal();
        }
})*/

/*Option Two Key press function - using javascript   using &&*/

document.addEventListener('keydown', function (e) {
    //console.log(e.key)

    if (e.key === 'Escape' && !modal.classList.contains('hidden')) {
        closeAllModal();
    }
})